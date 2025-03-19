using Content.Shared.Popups;
using Robust.Shared.Random;

namespace Content.Server._NC.AdvancedSpawner;

/// <summary>
/// System that initializes and executes advanced random spawning logic.
/// </summary>
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
        Sawmill.Debug($"[AdvancedSpawner] Initializing entity {uid}");

        var config = AdvancedRandomSpawnerConfig.FromComponent(component);

        foreach (var modifierData in EntityManager.GetComponents<SpawnModifierComponent>(uid))
        {
            Sawmill.Debug($"[AdvancedSpawner] Applying modifiers from SpawnModifierComponent on {uid}");

            config.ApplyModifiers(modifierData.WeightModifiers, modifierData.ExtraPrototypes);
            EntityManager.RemoveComponent<SpawnModifierComponent>(uid);
        }

        SpawnEntities(uid, config);
    }

    private void SpawnEntities(EntityUid uid, AdvancedRandomSpawnerConfig config)
    {
        var spawnCoords = Transform(uid).Coordinates;

        var spawner = AdvancedEntitySpawner.Create(_random, EntityManager, _popupSystem, config.Categories, config.MaxSpawnCount);
        var spawnedItems = spawner.SpawnEntities(uid, spawnCoords, config.Offset, config);

        Sawmill.Debug(spawnedItems.Count > 0
            ? $"[AdvancedSpawner] Entity {uid} spawned the following: {string.Join(", ", spawnedItems)}"
            : $"[AdvancedSpawner] Entity {uid} spawned nothing.");

        if (config.DeleteAfterSpawn)
        {
            Sawmill.Debug($"[AdvancedSpawner] Deleting spawner entity {uid} after spawn");
            EntityManager.QueueDeleteEntity(uid);
        }
    }
}
