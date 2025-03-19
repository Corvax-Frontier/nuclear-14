using Content.Server._NC.AdvancedSpawner;

namespace Content.Server._NC.ResourceGatheringSystem;
[RegisterComponent]
public sealed partial class ResourceGatheringComponent : Component
{
    [DataField] public float GatherTime { get; set; } = 5f;

    [DataField] public Dictionary<string, int> WeightModifiers { get; set; } = new();

    [DataField] public Dictionary<string, List<SpawnEntry>> ExtraPrototypes { get; set; } = new();
}
