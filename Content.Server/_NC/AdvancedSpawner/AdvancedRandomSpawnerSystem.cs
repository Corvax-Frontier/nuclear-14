using Content.Shared.Popups;
using Robust.Shared.Random;

namespace Content.Server._NC.AdvancedSpawner;

public sealed class AdvancedRandomSpawnerSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    private static readonly ISawmill Sawmill = Logger.GetSawmill("advancedSpawner");

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<AdvancedRandomSpawnerComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(EntityUid uid, AdvancedRandomSpawnerComponent component, MapInitEvent args)
    {
        Sawmill.Debug($"[AdvancedSpawner] MapInit started for entity {uid}");

        var config = AdvancedRandomSpawnerConfig.FromComponent(component);
        ApplyModifiersFromComponents(uid, config);
        SpawnEntitiesUsingSpawner(uid, config);
    }

    private void ApplyModifiersFromComponents(EntityUid uid, AdvancedRandomSpawnerConfig config)
    {
        foreach (var modifierData in EntityManager.GetComponents<SpawnModifierComponent>(uid))
        {
            Sawmill.Debug($"[AdvancedSpawner] Applying modifier from {uid}");
            config.ApplyModifiers(modifierData.WeightModifiers, modifierData.ExtraPrototypes);
            EntityManager.RemoveComponent<SpawnModifierComponent>(uid);
        }
    }

    public void SpawnEntitiesUsingSpawner(EntityUid uid, AdvancedRandomSpawnerConfig config)
    {
        var spawnCoords = Transform(uid).Coordinates;
        var spawner = AdvancedEntitySpawner.Create(_random, EntityManager, _popupSystem, config.Categories, config.MaxSpawnCount);
        var spawnedItems = spawner.SpawnEntities(uid, spawnCoords, config.Offset, config);

        Sawmill.Debug(spawnedItems.Count == 0
    ? $"[AdvancedSpawner] No entities spawned for {uid}"
    : $"[AdvancedSpawner] Spawned entities for {uid}: {string.Join(", ", spawnedItems)}");

        if (config.DeleteAfterSpawn)
        {
            Sawmill.Debug($"[AdvancedSpawner] Deleting spawner entity {uid} after spawn.");
            EntityManager.QueueDeleteEntity(uid);
        }
    }
}
