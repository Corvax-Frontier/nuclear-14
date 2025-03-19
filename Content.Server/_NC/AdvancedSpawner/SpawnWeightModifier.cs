namespace Content.Server._NC.AdvancedSpawner;

/// <summary>
/// Utility class for applying spawn weight and prototype modifiers.
/// </summary>
public static class SpawnWeightModifier
{
    private static readonly ISawmill Sawmill = Logger.GetSawmill("advancedSpawner");

    /// <summary>
    /// Applies weight changes and adds extra prototypes into existing collections.
    /// </summary>
    public static void ApplyModifiers(
        Dictionary<string, int> categoryWeights,
        Dictionary<string, List<SpawnEntry>> prototypes,
        Dictionary<string, int> weightModifiers,
        Dictionary<string, List<SpawnEntry>> extraPrototypes)
    {
        foreach (var (category, modifier) in weightModifiers)
        {
            if (categoryWeights.TryGetValue(category, out var currentWeight))
            {
                categoryWeights[category] = Math.Max(1, currentWeight + modifier);
                Sawmill.Debug($"[SpawnWeightModifier] Modified weight of '{category}' by {modifier}, new weight: {categoryWeights[category]}");
            }
        }

        foreach (var category in categoryWeights.Keys)
        {
            if (!prototypes.ContainsKey(category))
            {
                prototypes[category] = new List<SpawnEntry>();
                Sawmill.Debug($"[SpawnWeightModifier] Initialized prototype list for missing category '{category}'");
            }
        }

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
