using System.Linq;
using System.Numerics;
using Content.Shared.Popups;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server._NC.AdvancedSpawner
{
    /// <summary>
    /// Responsible for spawning entities based on weighted categories and prototypes.
    /// </summary>
    public sealed class AdvancedEntitySpawner
    {
        private readonly IRobustRandom _random;
        private readonly IEntityManager _entityManager;
        private readonly SharedPopupSystem _popupSystem;

        private int _spawnCount;
        private readonly List<SpawnCategory> _categories;
        private readonly int _maxSpawnCount;

        private static readonly ISawmill Sawmill = Logger.GetSawmill("advancedSpawner");

        private AdvancedEntitySpawner(IRobustRandom random, IEntityManager entityManager, SharedPopupSystem popupSystem, List<SpawnCategory> categories, int maxSpawnCount)
        {
            _random = random;
            _entityManager = entityManager;
            _popupSystem = popupSystem;
            _categories = categories;
            _maxSpawnCount = maxSpawnCount;
        }

        public static AdvancedEntitySpawner Create(IRobustRandom random, IEntityManager entityManager, SharedPopupSystem popupSystem, List<SpawnCategory> categories, int maxSpawnCount)
        {
            return new AdvancedEntitySpawner(random, entityManager, popupSystem, categories, maxSpawnCount);
        }

        /// <summary>
        /// Spawns entities based on the configuration and returns the spawned prototype IDs.
        /// </summary>
        public List<string> SpawnEntities(EntityUid spawnerUid, EntityCoordinates spawnCoords, float offset, AdvancedRandomSpawnerConfig config)
        {
            var spawnedItems = new List<string>();
            _spawnCount = 0;

            while (_spawnCount < _maxSpawnCount)
            {
                if (!TrySelectCategory(config, out var category))
                    break;

                if (!TrySelectPrototype(category, out var prototype))
                    continue;

                SpawnPrototype(prototype, spawnCoords, offset, spawnedItems);

                // Re-calculate chance to continue spawning
                if (!ShouldContinueSpawning(category, config))
                    break;
            }

            LogSpawns(spawnerUid, spawnCoords, spawnedItems);

            return spawnedItems;
        }

        #region Category & Prototype Selection

        private bool TrySelectCategory(AdvancedRandomSpawnerConfig config, out SpawnCategory selectedCategory)
        {
            var totalWeight = _categories.Sum(c => c.Weight + config.GetCategoryModifier(c.Name));
            if (totalWeight <= 0)
            {
                selectedCategory = _categories.First();
                return false;
            }

            var roll = _random.Next(totalWeight);
            var cumulative = 0;

            foreach (var category in _categories)
            {
                var adjustedWeight = category.Weight + config.GetCategoryModifier(category.Name);
                cumulative += adjustedWeight;
                if (roll < cumulative)
                {
                    selectedCategory = category;
                    return true;
                }
            }

            selectedCategory = _categories.First();
            return true;
        }

        private bool TrySelectPrototype(SpawnCategory category, out SpawnEntry prototype)
        {
            var entries = category.Prototypes;
            if (entries.Count == 0)
            {
                prototype = null!;
                return false;
            }

            var totalWeight = entries.Sum(p => p.Weight);
            var roll = _random.Next(totalWeight);
            var cumulative = 0;

            foreach (var entry in entries)
            {
                cumulative += entry.Weight;
                if (roll < cumulative)
                {
                    prototype = entry;
                    return true;
                }
            }

            prototype = entries.First();
            return true;
        }

        #endregion

        #region Spawning Logic

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

        #endregion

        #region Spawn Control & Logging

        private bool ShouldContinueSpawning(SpawnCategory category, AdvancedRandomSpawnerConfig config)
        {
            if (_spawnCount >= _maxSpawnCount)
                return false;

            var chance = CalculateSpawnChance(category, config);
            return _random.NextDouble() < chance;
        }

        private double CalculateSpawnChance(SpawnCategory category, AdvancedRandomSpawnerConfig config)
        {
            var adjustedWeight = Math.Max(1, category.Weight + config.GetCategoryModifier(category.Name));
            var totalWeight = _categories.Sum(c => Math.Max(1, c.Weight + config.GetCategoryModifier(c.Name)));
            if (totalWeight == 0)
                return 0.0;

            var baseChance = (double)adjustedWeight / totalWeight;
            var diminishingFactor = Math.Pow(1.0 - (double)_spawnCount / _maxSpawnCount, 2);

            // Gradually reduces the chance of continued spawning
            return baseChance * diminishingFactor;
        }

        private void LogSpawns(EntityUid spawnerUid, EntityCoordinates coords, List<string> spawnedItems)
        {
            if (spawnedItems.Count == 0)
            {
                Sawmill.Debug($"[AdvancedSpawner] No entities spawned at {coords}");
                return;
            }

            Sawmill.Debug($"[AdvancedSpawner] Spawned entities at {coords}: {string.Join(", ", spawnedItems)}");

            foreach (var proto in spawnedItems)
            {
                _popupSystem.PopupEntity($"Spawned item: {proto}", spawnerUid);
            }
        }

        #endregion
    }

    public static class SpawnerExtensions
    {
        /// <summary>
        /// Helper to get category modifier or zero if missing.
        /// </summary>
        public static int GetCategoryModifier(this AdvancedRandomSpawnerConfig config, string category)
        {
            return config.CategoryWeights.TryGetValue(category, out var modifier) ? modifier : 0;
        }
    }
}
