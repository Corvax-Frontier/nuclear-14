namespace Content.Server._NC.ResourceGatheringSystem;

[RegisterComponent]
public sealed partial class ServerResourceNodeComponent : Component
{
    [DataField] public float CooldownAfterGather = 600f;
    [DataField] internal float TimeBeforeNextGather;
    [DataField] public List<string> AllowedTools { get; set; } = new();
    [DataField] public string LootSpawner { get; set; } = "RandomResourceSpawner";
    [DataField] public string ResourceRichness = "medium"; // "rich", "medium", "poor"
}
