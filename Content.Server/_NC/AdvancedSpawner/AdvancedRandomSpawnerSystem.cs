using Robust.Shared.Random;
using Robust.Shared.Map;
using System.Linq;
using System.Numerics;

namespace Content.Server._NC.AdvancedSpawner;

/// <summary>
/// Система управления продвинутым рандомным спавнером.
/// </summary>
public sealed class AdvancedRandomSpawnerSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    private static readonly ISawmill Sawmill = Logger.GetSawmill("advancedSpawner");

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<AdvancedRandomSpawnerComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(EntityUid uid, AdvancedRandomSpawnerComponent component, MapInitEvent args)
    {
        Sawmill.Debug($"[AdvancedSpawner] MapInit started for entity {uid}");
        BuildAndSpawn(uid);
    }

    public bool BuildAndSpawn(EntityUid spawnerUid)
    {
        if (!TryComp<AdvancedRandomSpawnerComponent>(spawnerUid, out var spawnerComp))
        {
            Sawmill.Warning($"[AdvancedSpawner] Entity {spawnerUid} is missing AdvancedRandomSpawnerComponent.");
            return false;
        }

        var config = AdvancedRandomSpawnerConfig.FromComponent(spawnerComp);

        ApplyModifiersFromComponents(spawnerUid, config);

        config.RebuildCategories();

        CleanUpModifiers(spawnerUid);

        SpawnEntitiesUsingSpawner(spawnerUid, config);

        return true;
    }

    private void ApplyModifiersFromComponents(EntityUid uid, AdvancedRandomSpawnerConfig config)
    {
        var modifiers = EntityManager.GetComponents<SpawnModifierComponent>(uid).ToList();

        foreach (var modifier in modifiers)
        {
            Sawmill.Debug($"[AdvancedSpawner] Applying SpawnModifierComponent on {uid}");
            modifier.ApplyToConfig(config);
        }
    }

    private void CleanUpModifiers(EntityUid uid)
    {
        foreach (var unused in EntityManager.GetComponents<SpawnModifierComponent>(uid).ToList())
        {
            EntityManager.RemoveComponent<SpawnModifierComponent>(uid);
            Sawmill.Debug($"[AdvancedSpawner] Removed SpawnModifierComponent from {uid}");
        }
    }

    public void SpawnEntitiesUsingSpawner(EntityUid uid, AdvancedRandomSpawnerConfig config)
    {
        var categories = config.GetCategories();

        if (categories.Count == 0)
        {
            Sawmill.Warning($"[AdvancedSpawner] No categories defined for spawner {uid}. Aborting spawn.");
            return;
        }

        var spawnCoords = new EntityCoordinates(uid, Vector2.Zero);
        if (EntityManager.TryGetComponent(uid, out TransformComponent? transform))
        {
            spawnCoords = transform.Coordinates;
        }

        var spawner = AdvancedEntitySpawner.Create(_random, EntityManager, categories.ToList(), config.MaxSpawnCount);

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
