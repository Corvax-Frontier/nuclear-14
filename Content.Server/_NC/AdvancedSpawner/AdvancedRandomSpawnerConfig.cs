namespace Content.Server._NC.AdvancedSpawner;

public sealed class AdvancedRandomSpawnerConfig
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
    /// Adds a prototype to a specific category.
    /// </summary>
    public void AddPrototype(string category, SpawnEntry entry)
    {
        if (!Prototypes.ContainsKey(category))
            Prototypes[category] = new();

        Prototypes[category].Add(entry);
        Sawmill.Debug($"[AdvancedSpawnerConfig] Added prototype '{entry.PrototypeId}' to category '{category}'");
    }

    /// <summary>
    /// Applies weight changes and extra prototypes. Supports dynamic category creation.
    /// </summary>
    public void ApplyModifiers(Dictionary<string, int> weightModifiers, Dictionary<string, List<SpawnEntry>>? extraPrototypes = null)
    {
        ApplyWeightModifiers(weightModifiers);
        ApplyExtraPrototypes(extraPrototypes);
    }

    private void ApplyWeightModifiers(Dictionary<string, int> weightModifiers)
    {
        foreach (var (category, modifier) in weightModifiers)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                Sawmill.Warning("[AdvancedSpawnerConfig] Skipping weight modifier with empty category name.");
                continue;
            }

            if (CategoryWeights.TryGetValue(category, out var currentWeight))
            {
                CategoryWeights[category] = Math.Max(1, currentWeight + modifier);
                Sawmill.Debug($"[AdvancedSpawnerConfig] Modified weight of '{category}' by {modifier}, new weight: {CategoryWeights[category]}");
            }
            else
            {
                // Динамически создаём новую категорию с весом из модификатора
                var newWeight = Math.Max(1, modifier);
                CategoryWeights[category] = newWeight;
                Prototypes[category] = new List<SpawnEntry>();
                Sawmill.Debug($"[AdvancedSpawnerConfig] Created new category '{category}' with weight {newWeight} via modifier");
            }
        }
    }

    private void ApplyExtraPrototypes(Dictionary<string, List<SpawnEntry>>? extraPrototypes)
    {
        if (extraPrototypes == null)
            return;

        foreach (var (category, entries) in extraPrototypes)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                Sawmill.Warning("[AdvancedSpawnerConfig] Skipping extra prototypes with empty category name.");
                continue;
            }

            if (!Prototypes.TryGetValue(category, out var list))
            {
                // Если категория новая, создаём её с дефолтным весом 1
                list = new List<SpawnEntry>();
                Prototypes[category] = list;

                if (!CategoryWeights.ContainsKey(category))
                {
                    CategoryWeights.TryAdd(category, 1);
                    Sawmill.Debug($"[AdvancedSpawnerConfig] Auto-created category '{category}' with default weight 1 for extra prototypes");
                }

                Sawmill.Debug($"[AdvancedSpawnerConfig] Initialized prototype list for new category '{category}'");
            }

            foreach (var entry in entries)
            {
                list.Add(entry);
                Sawmill.Debug($"[AdvancedSpawnerConfig] Added prototype '{entry.PrototypeId}' to category '{category}'");
            }
        }
    }

    /// <summary>
    /// Пересобирает список категорий после применения всех модификаторов и изменений.
    /// Обязательно вызывать перед финальным спавном!
    /// </summary>
    public void RebuildCategories()
    {
        Categories.Clear();
        foreach (var category in CategoryWeights.Keys)
        {
            var entries = Prototypes.GetValueOrDefault(category, new List<SpawnEntry>());
            Categories.Add(new SpawnCategory(category, CategoryWeights[category], entries));
        }
        Sawmill.Debug("[AdvancedSpawnerConfig] Rebuilt Categories after modifiers.");
    }
}
