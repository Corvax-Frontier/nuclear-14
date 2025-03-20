namespace Content.Shared._NC.ResourceGatheringSystem;

[RegisterComponent]
public sealed partial class SharedResourceNodeComponent : Component
{
    [DataField] public float CooldownAfterGather = 600f;
    [DataField] public List<string> AllowedTools { get; set; } = new();
    [DataField] public string LootSpawner { get; set; } = "RandomResourceSpawner";
    [DataField] public ResourceRichness ResourceRichness = ResourceRichness.Medium;
    [DataField] public float TimeBeforeNextGather;
}
