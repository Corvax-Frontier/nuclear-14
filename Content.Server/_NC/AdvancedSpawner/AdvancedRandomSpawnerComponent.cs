namespace Content.Server._NC.AdvancedSpawner;

[RegisterComponent]
public sealed partial class AdvancedRandomSpawnerComponent : Component
{
    [DataField] public Dictionary<string, int> CategoryWeights = new();
    [DataField] public Dictionary<string, List<SpawnEntry>> PrototypeLists = new();
    [DataField] public float Offset = 0.2f;
    [DataField] public bool DeleteAfterSpawn = true;
    [DataField] public int MaxSpawnCount = 3;
}
