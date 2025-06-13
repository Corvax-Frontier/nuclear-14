namespace Content.Server._NC.AdvancedSpawner;

/// <summary>
/// Represents a spawn category with its weight and list of possible prototypes.
/// </summary>
public sealed class SpawnCategory(string name, int weight, List<SpawnEntry> prototypes)
{
    public string Name { get; } = name;
    public int Weight { get; set; } = weight;
    public List<SpawnEntry> Prototypes { get; } = prototypes;
}
