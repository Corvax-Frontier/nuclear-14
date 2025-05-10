using Robust.Shared.Serialization;

namespace Content.Shared._NC.ResourceGatheringSystem;

/// <summary>
/// Richness level of a resource node, affecting loot chances.
/// </summary>
[Serializable, NetSerializable]
public enum ResourceRichness
{
    /// <summary>
    /// Poor node, reduces chance of rare drops.
    /// </summary>
    Poor,

    /// <summary>
    /// Medium node, neutral chances.
    /// </summary>
    Medium,

    /// <summary>
    /// Rich node, increases chance of rare drops.
    /// </summary>
    Rich
}
