namespace Content.Server._NC.AdvancedSpawner;

/// <summary>
/// Configuration container for advanced random spawner. Holds categories, weights, and prototype lists.
/// </summary>
public class AdvancedRandomSpawnerConfig
{
    public readonly Dictionary<string, int> CategoryWeights;
    public readonly List<SpawnCategory> Categories;
    public readonly Dictionary<string, List<SpawnEntry>> Prototypes;

    public readonly float Offset;
    public readonly bool DeleteAfterSpawn;
    public readonly int MaxSpawnCount;

    private static readonly ISawmill Sawmill = Logger.GetSawmill("advancedSpawner");

    public AdvancedRandomSpawnerConfig(AdvancedRandomSpawnerComponent comp)
    {
        CategoryWeights = new(comp.CategoryWeights);
        Categories = new();
        Prototypes = new();

        foreach (var category in comp.CategoryWeights.Keys)
        {
            var entries = comp.PrototypeLists.GetValueOrDefault(category, new List<SpawnEntry>());
            Prototypes[category] = new(entries);
            Categories.Add(new SpawnCategory(category, CategoryWeights[category], Prototypes[category]));
        }

        Offset = comp.Offset;
        DeleteAfterSpawn = comp.DeleteAfterSpawn;
        MaxSpawnCount = comp.MaxSpawnCount;

        Sawmill.Debug("[AdvancedSpawnerConfig] Initialized from component.");
    }

    public static AdvancedRandomSpawnerConfig FromComponent(AdvancedRandomSpawnerComponent comp)
    {
        return new AdvancedRandomSpawnerConfig(comp);
    }

    /// <summary>
    /// Applies dynamic modifiers to category weights and prototypes.
    /// </summary>
    public void ApplyModifiers(Dictionary<string, int> weightModifiers, Dictionary<string, List<SpawnEntry>> extraPrototypes)
    {
        foreach (var (category, modifier) in weightModifiers)
        {
            if (CategoryWeights.TryGetValue(category, out var current))
            {
                CategoryWeights[category] = Math.Max(1, current + modifier);
                Sawmill.Debug($"[AdvancedSpawnerConfig] Modified weight of '{category}' by {modifier}, new weight: {CategoryWeights[category]}");
            }
        }

        foreach (var category in CategoryWeights.Keys)
        {
            if (!Prototypes.ContainsKey(category))
                Prototypes[category] = new List<SpawnEntry>();
        }

        foreach (var (category, extraEntries) in extraPrototypes)
        {
            if (!Prototypes.ContainsKey(category))
                Prototypes[category] = new List<SpawnEntry>();

            foreach (var entry in extraEntries)
            {
                if (Prototypes[category].Exists(e => e.PrototypeId == entry.PrototypeId))
                    continue;
                Prototypes[category].Add(entry);
                Sawmill.Debug($"[AdvancedSpawnerConfig] Added extra prototype '{entry.PrototypeId}' to category '{category}'");
            }
        }
    }
}

