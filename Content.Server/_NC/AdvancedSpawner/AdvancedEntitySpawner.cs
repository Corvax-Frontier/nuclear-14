using System.Linq;
using System.Numerics;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server._NC.AdvancedSpawner;

public sealed class AdvancedEntitySpawner
{
    private readonly IRobustRandom _random;
    private readonly IEntityManager _entityManager;

    private int _spawnCount;
    private readonly List<SpawnCategory> _categories;
    private readonly int _maxSpawnCount;

    private static readonly ISawmill Sawmill = Logger.GetSawmill("advancedSpawner");

    private AdvancedEntitySpawner(IRobustRandom random, IEntityManager entityManager, List<SpawnCategory> categories, int maxSpawnCount)
    {
        _random = random;
        _entityManager = entityManager;
        _categories = categories;
        _maxSpawnCount = maxSpawnCount;
    }

    public static AdvancedEntitySpawner Create(IRobustRandom random, IEntityManager entityManager, List<SpawnCategory> categories, int maxSpawnCount)
    {
        return new AdvancedEntitySpawner(random, entityManager, categories, maxSpawnCount);
    }

    public List<string> SpawnEntities(EntityUid spawnerUid, EntityCoordinates spawnCoords, float offset, AdvancedRandomSpawnerConfig config)
    {
        var spawnedItems = new List<string>();
        _spawnCount = 0;

        while (_spawnCount < _maxSpawnCount)
        {
            if (!TrySelectCategory(config, out var category))
            {
                Sawmill.Warning("[AdvancedSpawner] Failed to select a valid category. Stopping spawn.");
                break;
            }

            if (!TrySelectPrototype(category, out var prototype))
            {
                Sawmill.Debug($"[AdvancedSpawner] No valid prototype found in category '{category.Name}'");
                continue;
            }

            SpawnPrototype(prototype, spawnCoords, offset, spawnedItems);

            if (!ShouldContinueSpawning(category, config))
                break;
        }

        LogSpawns(spawnCoords, spawnedItems);
        return spawnedItems;
    }

    private bool TrySelectCategory(AdvancedRandomSpawnerConfig config, out SpawnCategory selectedCategory)
    {
        var totalWeight = _categories.Sum(c => Math.Max(0, c.Weight + config.GetCategoryWeight(c.Name)));

        if (totalWeight <= 0)
        {
            selectedCategory = null!;
            return false;
        }

        var roll = _random.Next(totalWeight);
        var cumulative = 0;

        foreach (var category in _categories)
        {
            var adjustedWeight = Math.Max(0, category.Weight + config.GetCategoryWeight(category.Name));
            cumulative += adjustedWeight;
            if (roll < cumulative)
            {
                selectedCategory = category;
                return true;
            }
        }

        selectedCategory = null!;
        return false;
    }

    private bool TrySelectPrototype(SpawnCategory category, out SpawnEntry prototype)
    {
        var entries = category.Prototypes;
        if (entries.Count == 0)
        {
            prototype = null!;
            return false;
        }

        var totalWeight = entries.Sum(p => Math.Max(0, p.Weight));
        if (totalWeight <= 0)
        {
            prototype = null!;
            return false;
        }

        var roll = _random.Next(totalWeight);
        var cumulative = 0;

        foreach (var entry in entries)
        {
            cumulative += Math.Max(0, entry.Weight);
            if (roll < cumulative)
            {
                prototype = entry;
                return true;
            }
        }

        prototype = null!;
        return false;
    }

    private void SpawnPrototype(SpawnEntry prototype, EntityCoordinates spawnCoords, float offset, List<string> spawnedItems)
    {
        for (var i = 0; i < prototype.Count && _spawnCount < _maxSpawnCount; i++)
        {
            SpawnEntity(prototype.PrototypeId, spawnCoords, offset);
            spawnedItems.Add(prototype.PrototypeId);
            _spawnCount++;
        }
    }

    private void SpawnEntity(string prototypeId, EntityCoordinates spawnCoords, float offset)
    {
        var displacement = GetRandomDisplacement(offset);
        var newCoords = spawnCoords.WithPosition(spawnCoords.Position + displacement);
        _entityManager.SpawnEntity(prototypeId, newCoords);
    }

    private Vector2 GetRandomDisplacement(float offset)
    {
        var angle = _random.NextFloat() * MathF.Tau;
        var radius = _random.NextFloat() * offset;
        return new Vector2(MathF.Cos(angle) * radius, MathF.Sin(angle) * radius);
    }

    private bool ShouldContinueSpawning(SpawnCategory category, AdvancedRandomSpawnerConfig config)
    {
        if (_spawnCount >= _maxSpawnCount)
            return false;

        var chance = CalculateSpawnChance(category, config);
        return _random.NextDouble() < chance;
    }

    private double CalculateSpawnChance(SpawnCategory category, AdvancedRandomSpawnerConfig config)
    {
        var adjustedWeight = Math.Max(0, category.Weight + config.GetCategoryWeight(category.Name));
        var totalWeight = _categories.Sum(c => Math.Max(0, c.Weight + config.GetCategoryWeight(c.Name)));

        if (totalWeight == 0)
            return 0.0;

        var baseChance = (double)adjustedWeight / totalWeight;
        var diminishingFactor = Math.Pow(1.0 - (double)_spawnCount / _maxSpawnCount, 2);

        return baseChance * diminishingFactor;
    }

    private void LogSpawns(EntityCoordinates coords, List<string> spawnedItems)
    {
        if (spawnedItems.Count == 0)
        {
            Sawmill.Debug($"[AdvancedSpawner] No entities spawned at {coords}");
            return;
        }

        Sawmill.Debug($"[AdvancedSpawner] Spawned {spawnedItems.Count} entities at {coords}: {string.Join(", ", spawnedItems)}");
    }
}
