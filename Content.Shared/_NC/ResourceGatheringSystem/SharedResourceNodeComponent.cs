namespace Content.Shared._NC.ResourceGatheringSystem;

/// <summary>
/// Component representing a resource node from which players can gather resources.
/// </summary>
[RegisterComponent]
public sealed partial class ResourceNodeComponent : Component
{
    /// <summary>
    /// List of allowed tool IDs that can be used to gather from this node.
    /// </summary>
    [DataField("allowedTools")]
    [ViewVariables]
    public List<string> AllowedTools = new();

    /// <summary>
    /// The richness of the resource node, affecting the chance of rare drops.
    /// </summary>
    [DataField("richness")]
    [ViewVariables]
    public ResourceRichness ResourceRichness = ResourceRichness.Medium;

    /// <summary>
    /// The loot spawner prototype ID used to generate resources from this node.
    /// </summary>
    [DataField("lootSpawner")]
    [ViewVariables]
    public string LootSpawner = "RandomResourceSpawner";

    /// <summary>
    /// The cooldown time (in seconds) before the node can be gathered again.
    /// </summary>
    [DataField("cooldown")]
    [ViewVariables]
    public float CooldownAfterGather = 600f;

    /// <summary>
    /// The remaining time (in seconds) until the node is ready for the next gathering attempt.
    /// </summary>
    [DataField("timeRemaining")]
    [ViewVariables]
    public float TimeBeforeNextGather;
}
