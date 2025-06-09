using Content.Shared._NC.Trade;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;

namespace Content.Server._NC.Trade;

/// <summary>
/// Серверная система обработки Bound UI взаимодействия с магазином.
/// </summary>
public sealed class NcStoreSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entMan = default!;
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly IEntitySystemManager _sysMan = default!;

    public override void Initialize()
    {
        // Подписка на событие от Bound UI
        SubscribeLocalEvent<NcStoreComponent, StoreBuyListingBoundUiMessage>(OnBuyRequest);
    }

    private void OnBuyRequest(EntityUid uid, NcStoreComponent comp, StoreBuyListingBoundUiMessage msg)
    {
        // Проверка сессии игрока
        var actor = msg.Actor;
        if (!_entMan.EntityExists(actor))
            return;

        if (!_entMan.TryGetComponent(uid, out TransformComponent? storeXform) ||
            !_entMan.TryGetComponent(actor, out TransformComponent? userXform))
            return;

        // Проверка дистанции между пользователем и магазином
        if (!_transform.InRange(storeXform.Coordinates, userXform.Coordinates, 3f))
            return;

        // Попытка покупки
        var logic = _sysMan.GetEntitySystem<NcStoreLogicSystem>();
        logic.TryPurchase(msg.ListingId, uid, comp, actor);
    }
}
