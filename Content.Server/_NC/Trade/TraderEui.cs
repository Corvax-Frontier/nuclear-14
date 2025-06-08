using Content.Server.EUI;
using Content.Shared._NC.Trade;
using Content.Shared.Eui;

namespace Content.Server._NC.Trade;

public sealed class TraderEui : BaseEui
{
    [Dependency] private readonly IEntitySystemManager _entitySys = default!;
    [Dependency] private readonly IEntityManager _entMan = default!;

    public override void HandleMessage(EuiMessageBase msg)
    {
        if (msg is not StoreBuyListingEuiMessage buy)
            return;

        if (Player.AttachedEntity is not { Valid: true } user)
            return;

        // Найдём ближайший магазин с компонентом NcStoreComponent (например, радиус 3 тайла)
        NcStoreComponent? foundComp = null;
        EntityUid machine = EntityUid.Invalid;

        foreach (var (uid, comp) in _entMan.EntityQuery<NcStoreComponent>())
        {
            if (!_entMan.TryGetComponent(uid, out TransformComponent? xform) ||
                !_entMan.TryGetComponent(user, out TransformComponent? userXform))
                continue;

            if (!xform.Coordinates.InRange(_entMan, userXform.Coordinates, 3f))
                continue;

            foundComp = comp;
            machine = uid;
            break;
        }

        if (foundComp == null || !_entMan.EntityExists(machine))
            return;

        var logic = _entitySys.GetEntitySystem<NcStoreLogicSystem>();
        logic.TryPurchase(new(buy.ListingId), machine, foundComp, user);
    }
}
