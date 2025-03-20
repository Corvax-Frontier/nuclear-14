namespace Content.Shared._NC.ResourceGatheringSystem;

/// <summary>
/// Component that stores all tool-related data for resource gathering.
/// Defines tool effects, modifiers, and additional spawn logic.
/// </summary>
[RegisterComponent]
public sealed partial class SharedResourceToolComponent : Component
{
    /// <summary>
    /// Unique tool ID used for checking compatibility with resource nodes.
    /// </summary>
    [DataField("toolId")]
    public string ToolId = string.Empty;

    /// <summary>
    /// Modifies the weight of specific loot categories when gathering resources.
    /// Example: { "rare": 5, "common": -2 }
    /// </summary>
    [DataField("weightModifiers")]
    public Dictionary<string, int> WeightModifiers = new();

    /// <summary>
    /// Adds specific prototype IDs to spawn if this tool is used.
    /// Example: { "rare": ["gold_chunk", "diamond_piece"] }
    /// </summary>
    [DataField("extraPrototypes")]
    public Dictionary<string, List<string>> ExtraPrototypeIds = new();

    /// <summary>
    /// Removes specific prototype IDs from the potential spawn pool.
    /// Example: { "common": ["stone_chunk"] }
    /// </summary>
    [DataField("removePrototypes")]
    public Dictionary<string, List<string>> RemovePrototypes = new();

    /// <summary>
    /// Adds entirely new loot categories with a specific weight.
    /// Example: { "legendary": 10 }
    /// </summary>
    [DataField("addCategories")]
    public Dictionary<string, int> AddCategories = new();

    /// <summary>
    /// Removes entire loot categories when this tool is used.
    /// Example: [ "trash", "common" ]
    /// </summary>
    [DataField("removeCategories")]
    public List<string> RemoveCategories = new();
}
