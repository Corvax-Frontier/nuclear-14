using Robust.Shared.GameStates;

namespace Content.Server._NC.AdvancedSpawner;
/// <summary>
/// Temporary component that stores dynamic spawn category weight modifiers and additional prototype injections.
/// Can be added by any system to influence spawning logic.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class SpawnModifierComponent : Component
{
    /// <summary>
    /// Dynamic weight modifiers applied to spawn categories.
    /// Example: { "weapons": 5, "medkits": -2 }
    /// </summary>
    [DataField]
    public Dictionary<string, int> WeightModifiers = new();

    /// <summary>
    /// Extra prototypes to inject into specific spawn categories during spawning.
    /// Example: { "loot": [SpawnEntry(\"gold_proto\", 3, 1)] }
    /// </summary>
    [DataField]
    public Dictionary<string, List<SpawnEntry>> ExtraPrototypes = new();
}
