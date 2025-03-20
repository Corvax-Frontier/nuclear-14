namespace Content.Shared._NC.ResourceGatheringSystem;

[RegisterComponent]
public sealed partial class SharedResourceGatheringComponent : Component
{
    [DataField] public float GatherTime { get; set; } = 5f;

    [DataField] public Dictionary<string, int> WeightModifiers { get; set; } = new();

    [DataField]
    public Dictionary<string, List<string>> ExtraPrototypeIds { get; set; } = new();
}
