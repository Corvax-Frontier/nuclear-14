using Content.Server.Construction.Completions;
using Content.Shared.Humanoid;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Timing;
using Robust.Shared.Player;
using Content.Shared.SSDIndicator;

namespace Content.Server._NC14.Despawn;

public sealed class DespawnSystem : EntitySystem
{
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<DespawnComponent>();
        while (query.MoveNext(out var uid, out var despawnComponent))
        {
            var xformQuery = GetEntityQuery<TransformComponent>();

            if (despawnComponent.NextTimeToCheck > _gameTiming.CurTime)
                continue;

            if (!xformQuery.TryGetComponent(uid, out var xform) || xform.MapUid == null)
                continue;

            UpdateDespawnComponent(uid, despawnComponent, xform);
        }
    }

    private void UpdateDespawnComponent(EntityUid uid, DespawnComponent despawnComponent, TransformComponent xform)
    {
        var entitiesInRange = _lookup.GetEntitiesInRange<HumanoidAppearanceComponent>(xform.Coordinates, despawnComponent.DistanceToCheck);

        bool isSsdNear = false;
        bool isHumanoidNear = false;

        foreach (var entity in entitiesInRange)
        {
            if (entity.Owner == uid)
                continue;

            if (TryComp(entity, out SSDIndicatorComponent? ssdComponent))
            {
                if (ssdComponent.IsSSD)
                    isSsdNear = true;
                else
                    isHumanoidNear = true;
            }
        }

        despawnComponent.IsSsdNear = isSsdNear;
        despawnComponent.IsHumanoidNear = isHumanoidNear && !isSsdNear;

        HandleDespawnLogic(uid, despawnComponent);
    }

    private void HandleDespawnLogic(EntityUid uid, DespawnComponent despawnComponent)
    {
        if (despawnComponent.IsHumanoidNear)
        {
            if (despawnComponent.ReadyToDelete)
                despawnComponent.ReadyToDelete = false;

            despawnComponent.NextTimeToDelete = _gameTiming.CurTime + despawnComponent.DelayToDelete;
            despawnComponent.NextTimeToCheck = _gameTiming.CurTime + despawnComponent.DelayToCheck;
        }
        else if (!despawnComponent.ReadyToDelete)
        {
            despawnComponent.NextTimeToDelete = _gameTiming.CurTime + despawnComponent.DelayToDelete;
            despawnComponent.NextTimeToCheck = _gameTiming.CurTime + despawnComponent.DelayToCheck;
            despawnComponent.ReadyToDelete = true;
        }
        else if (despawnComponent.ReadyToDelete && despawnComponent.NextTimeToDelete < _gameTiming.CurTime)
        {
            EntityManager.DeleteEntity(uid);
        }
    }
}
