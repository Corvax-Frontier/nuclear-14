namespace Content.Server._NC.AdvancedSpawner;

/// <summary>
/// Represents a single spawnable prototype entry with weight and count.
/// </summary>
[DataDefinition]
public sealed partial class SpawnEntry
{
    /// <summary>
    /// Prototype ID to spawn.
    /// </summary>
    [DataField("id")]
    public string PrototypeId = string.Empty;

    /// <summary>
    /// Weight of this prototype in selection.
    /// </summary>
    [DataField]
    public int Weight = 1;

    /// <summary>
    /// Number of times this prototype should spawn per pick.
    /// </summary>
    [DataField]
    public int Count = 1;

    public SpawnEntry() { }

    public SpawnEntry(string prototypeId, int weight, int count)
    {
        PrototypeId = prototypeId;
        Weight = weight;
        Count = count;
    }
}
