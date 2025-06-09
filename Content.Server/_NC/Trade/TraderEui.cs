using Content.Shared._NC.Trade;
using Robust.Server.GameObjects;

namespace Content.Server._NC.Trade;

/// <summary>
/// Серверная система обработки Bound UI взаимодействия с магазином.
/// </summary>
public sealed class NcStoreSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entMan = null!;
    [Dependency] private readonly TransformSystem _transform = null!;
    [Dependency] private readonly IEntitySystemManager _sysMan = null!;

    private static readonly ISawmill Sawmill = Logger.GetSawmill("ncstore");

    public override void Initialize() =>
        SubscribeLocalEvent<NcStoreComponent, StoreBuyListingBoundUiMessage>(OnBuyRequest);

    private void OnBuyRequest(EntityUid uid, NcStoreComponent comp, StoreBuyListingBoundUiMessage msg)
    {
        var actor = msg.Actor;
        if (!_entMan.EntityExists(actor))
        {
            Sawmill.Warning($"[Buy] Actor entity {actor} does not exist.");
            return;
        }

        if (!_entMan.TryGetComponent(uid, out TransformComponent? storeXform))
        {
            Sawmill.Warning($"[Buy] No TransformComponent on store entity {ToPrettyString(uid)}.");
            return;
        }

        if (!_entMan.TryGetComponent(actor, out TransformComponent? userXform))
        {
            Sawmill.Warning($"[Buy] No TransformComponent on actor {ToPrettyString(actor)}.");
            return;
        }

        if (!_transform.InRange(storeXform.Coordinates, userXform.Coordinates, 3f))
        {
            Sawmill.Warning($"[Buy] User too far from store: {ToPrettyString(actor)} -> {ToPrettyString(uid)}.");
            return;
        }

        var logic = _sysMan.GetEntitySystem<NcStoreLogicSystem>();
        var success = logic.TryPurchase(msg.ListingId, uid, comp, actor);

        if (success)
        {
            Sawmill.Info($"[Buy] {ToPrettyString(actor)} купил '{msg.ListingId}' у {ToPrettyString(uid)}.");
            var uiSys = _sysMan.GetEntitySystem<StoreStructuredSystem>();
            uiSys.UpdateUiState(uid, comp, actor);
        }
        else
            Sawmill.Warning($"[Buy] Покупка не удалась: listingId={msg.ListingId}, user={ToPrettyString(actor)}.");
    }
}
