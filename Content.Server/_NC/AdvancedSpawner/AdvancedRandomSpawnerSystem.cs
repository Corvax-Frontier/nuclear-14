using Robust.Shared.Random;
using System.Linq;


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

        var config = AdvancedRandomSpawnerConfig.FromComponent(component);
        ApplyModifiersFromComponents(uid, config);

        config.RebuildCategories();

        SpawnEntitiesUsingSpawner(uid, config);
    }

    private void ApplyModifiersFromComponents(EntityUid uid, AdvancedRandomSpawnerConfig config)
    {
        var modifiers = EntityManager.GetComponents<SpawnModifierComponent>(uid).ToList();
        foreach (var modifierData in modifiers)
        {
            Sawmill.Debug($"[AdvancedSpawner] Applying modifier from entity {uid}");
            config.ApplyModifiers(modifierData.WeightModifiers, modifierData.ExtraPrototypes);
        }

        // Безопасно удаляем модификаторы отдельным проходом
        foreach (var mod in modifiers)
        {
            EntityManager.RemoveComponent<SpawnModifierComponent>(uid);
            Sawmill.Debug($"[AdvancedSpawner] Removed SpawnModifierComponent from {uid}");
        }
    }

    /// <summary>
    /// Выполняет спавн сущностей по сконфигурированному спавнеру.
    /// </summary>
    public void SpawnEntitiesUsingSpawner(EntityUid uid, AdvancedRandomSpawnerConfig config)
    {
        if (config.Categories.Count == 0)
        {
            Sawmill.Warning($"[AdvancedSpawner] No categories defined for spawner {uid}. Aborting spawn.");
            return;
        }

        var spawnCoords = Transform(uid).Coordinates;
        var spawner = AdvancedEntitySpawner.Create(_random, EntityManager, config.Categories, config.MaxSpawnCount);

        var spawnedItems = spawner.SpawnEntities(uid, spawnCoords, config.Offset, config);

        if (spawnedItems.Count == 0)
            Sawmill.Debug($"[AdvancedSpawner] No entities spawned for {uid}");
        else
            Sawmill.Debug($"[AdvancedSpawner] Spawned entities for {uid}: {string.Join(", ", spawnedItems)}");

        if (config.DeleteAfterSpawn)
        {
            Sawmill.Debug($"[AdvancedSpawner] Deleting spawner entity {uid} after spawn.");
            EntityManager.QueueDeleteEntity(uid);
        }
    }
}
