namespace Content.Shared._NC.ResourceGatheringSystem;

[RegisterComponent]
public sealed partial class SharedResourceToolComponent : Component
{
    [DataField] public string ToolId = string.Empty;

    [DataField] public Dictionary<string, int> WeightModifiers = new();

    [DataField]
    public Dictionary<string, List<string>> ExtraPrototypeIds = new();

    [DataField]
    public Dictionary<string, List<string>> RemovePrototypes = new();

    [DataField]
    public Dictionary<string, int> AddCategories = new();

    [DataField]
    public List<string> RemoveCategories = new();
}
