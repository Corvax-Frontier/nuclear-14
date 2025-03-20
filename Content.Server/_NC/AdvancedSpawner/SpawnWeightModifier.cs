namespace Content.Server._NC.AdvancedSpawner;

public static class SpawnWeightModifier
{
    private static readonly ISawmill Sawmill = Logger.GetSawmill("advancedSpawner");


    public static void ApplyModifiers(
        Dictionary<string, int> categoryWeights,
        Dictionary<string, List<SpawnEntry>> prototypes,
        Dictionary<string, int> weightModifiers,
        Dictionary<string, List<SpawnEntry>> extraPrototypes)
    {
        ApplyWeightModifiers(categoryWeights, weightModifiers);
        EnsurePrototypeCategories(categoryWeights, prototypes);
        ApplyExtraPrototypes(prototypes, extraPrototypes);
    }

    private static void ApplyWeightModifiers(
        Dictionary<string, int> categoryWeights,
        Dictionary<string, int> weightModifiers)
    {
        foreach (var (category, modifier) in weightModifiers)
        {
            if (categoryWeights.TryGetValue(category, out var currentWeight))
            {
                categoryWeights[category] = Math.Max(1, currentWeight + modifier);
                Sawmill.Debug($"[SpawnWeightModifier] Modified weight of '{category}' by {modifier}, new weight: {categoryWeights[category]}");
            }
        }
    }

    private static void EnsurePrototypeCategories(
        Dictionary<string, int> categoryWeights,
        Dictionary<string, List<SpawnEntry>> prototypes)
    {
        foreach (var category in categoryWeights.Keys)
        {
            if (!prototypes.ContainsKey(category))
            {
                prototypes[category] = new List<SpawnEntry>();
                Sawmill.Debug($"[SpawnWeightModifier] Initialized prototype list for missing category '{category}'");
            }
        }
    }

    private static void ApplyExtraPrototypes(
        Dictionary<string, List<SpawnEntry>> prototypes,
        Dictionary<string, List<SpawnEntry>> extraPrototypes)
    {
        foreach (var (category, extraEntries) in extraPrototypes)
        {
            if (!prototypes.ContainsKey(category))
            {
                prototypes[category] = new List<SpawnEntry>();
                Sawmill.Debug($"[SpawnWeightModifier] Initialized prototype list for extra category '{category}'");
            }

            foreach (var entry in extraEntries)
            {
                prototypes[category].Add(entry);
                Sawmill.Debug($"[SpawnWeightModifier] Added extra prototype '{entry.PrototypeId}' to category '{category}'");
            }
        }
    }
}
