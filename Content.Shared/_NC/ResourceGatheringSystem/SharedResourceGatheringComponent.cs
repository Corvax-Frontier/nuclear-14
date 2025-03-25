namespace Content.Shared._NC.ResourceGatheringSystem;

/// <summary>
/// Component that defines gathering behavior and modifiers for resource gathering tools.
/// </summary>
[RegisterComponent]
public sealed partial class SharedResourceGatheringComponent : Component
{
    /// <summary>
    /// Base time (in seconds) required to gather resources from a node.
    /// </summary>
    [DataField("gatherTime")]
    [ViewVariables(VVAccess.ReadWrite)]
    public float GatherTime = 5f;

    /// <summary>
    /// Modifiers that adjust spawn chances for specific loot categories.
    /// Example: { "rare": 2, "common": -1 }
    /// </summary>
    [DataField("weightModifiers")]
    [ViewVariables(VVAccess.ReadWrite)]
    public Dictionary<string, int> WeightModifiers = new();

    /// <summary>
    /// Additional prototype IDs that can be spawned after successful gathering.
    /// Example: { "rare": ["gold_nugget", "crystal_shard"] }
    /// </summary>
    [DataField("extraPrototypes")]
    [ViewVariables(VVAccess.ReadWrite)]
    public Dictionary<string, List<string>> ExtraPrototypeIds = new();
}
