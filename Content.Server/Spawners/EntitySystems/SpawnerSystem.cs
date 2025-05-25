using System.Threading;
using Content.Server.Spawners.Components;
using Content.Shared.Humanoid;
using Content.Shared.SSDIndicator;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Spawners.EntitySystems;

public sealed class SpawnerSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;

    private const float SpawnBlockRange = 15f;
    private EntityQuery<MetaDataComponent> _metaQuery;
    private EntityQuery<TransformComponent> _xformQuery;

    public override void Initialize()
    {
        base.Initialize();
        _metaQuery = GetEntityQuery<MetaDataComponent>();
        _xformQuery = GetEntityQuery<TransformComponent>();
        SubscribeLocalEvent<TimedSpawnerComponent, ComponentInit>(OnSpawnerInit);
        SubscribeLocalEvent<TimedSpawnerComponent, ComponentShutdown>(OnTimedSpawnerShutdown);
    }

    private void OnSpawnerInit(EntityUid uid, TimedSpawnerComponent component, ComponentInit args)
    {
        component.TokenSource = new CancellationTokenSource();
        uid.SpawnRepeatingTimer(
            TimeSpan.FromSeconds(component.IntervalSeconds),
            () => OnTimerFired(uid, component),
            component.TokenSource.Token
        );
    }

    private void OnTimerFired(EntityUid uid, TimedSpawnerComponent component)
    {
        if (ShouldBlockSpawn(uid, component))
            return;

        if (!_random.Prob(component.Chance))
            return;

        var xform = _xformQuery.GetComponent(uid);
        var coordinates = xform.Coordinates;

        for (var i = 0; i < _random.Next(component.MinimumEntitiesSpawned, component.MaximumEntitiesSpawned); i++)
        {
            var entity = _random.Pick(component.Prototypes);
            SpawnAtPosition(entity, coordinates);
        }
    }

    private bool ShouldBlockSpawn(EntityUid uid, TimedSpawnerComponent component)
    {
        if (!_xformQuery.TryGetComponent(uid, out var xform) || xform.MapUid == null)
            return true;

        // Проверка активных игроков
        foreach (var entity in _lookup.GetEntitiesInRange<HumanoidAppearanceComponent>(xform.MapPosition, SpawnBlockRange))
        {
            if (!Exists(entity.Owner)) continue; // Важная проверка
            if (entity.Owner == uid) continue;

            if (TryComp<SSDIndicatorComponent>(entity.Owner, out var ssd) && ssd.IsSSD)
                continue;

            return true;
        }

        // Проверка существующих мобов с защитой
        foreach (var entity in _lookup.GetEntitiesInRange(xform.MapPosition, SpawnBlockRange))
        {
            if (!Exists(entity)) continue;
            if (entity == uid) continue;

            if (!_metaQuery.TryGetComponent(entity, out var meta) ||
                meta.EntityPrototype?.ID is not { } prototypeId)
                continue;

            if (component.Prototypes.Contains(prototypeId))
                return true;
        }

        return false;
    }

    private void OnTimedSpawnerShutdown(EntityUid uid, TimedSpawnerComponent component, ComponentShutdown args)
    {
        component.TokenSource?.Cancel();
        component.TokenSource = null;
    }
}
