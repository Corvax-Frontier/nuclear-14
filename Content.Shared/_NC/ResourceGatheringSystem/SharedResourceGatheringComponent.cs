namespace Content.Shared._NC.ResourceGatheringSystem;

[RegisterComponent]
public sealed partial class SharedResourceGatheringComponent : Component
{
    /// <summary>
    /// Base time (in seconds) required to gather resources from a node.
    /// </summary>
    [DataField("gatherTime")]
    public float GatherTime = 5f;

    /// <summary>
    /// Modifiers that adjust spawn chances for specific loot categories.
    /// Example: { "rare": 2, "common": -1 }
    /// </summary>
    [DataField("weightModifiers")]
    public Dictionary<string, int> WeightModifiers = new();

    /// <summary>
    /// Additional prototype IDs that can be spawned after successful gathering.
    /// Example: { "rare": ["gold_nugget", "crystal_shard"] }
    /// </summary>
    [DataField("extraPrototypes")]
    public Dictionary<string, List<string>> ExtraPrototypeIds = new();
}
