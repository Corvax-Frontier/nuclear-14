namespace Content.Server._NC.AdvancedSpawner;

/// <summary>
/// Represents a spawn category with its weight and list of possible prototypes.
/// </summary>
public class SpawnCategory
{
    /// <summary>
    /// Unique name of the category.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Base weight of the category used during spawn selection.
    /// </summary>
    public int Weight { get; set; }

    /// <summary>
    /// List of prototype entries associated with this category.
    /// </summary>
    public List<SpawnEntry> Prototypes { get; }

    public SpawnCategory(string name, int weight, List<SpawnEntry> prototypes)
    {
        Name = name;
        Weight = weight;
        Prototypes = prototypes;
    }
}
