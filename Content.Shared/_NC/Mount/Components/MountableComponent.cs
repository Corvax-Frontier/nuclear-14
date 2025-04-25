using System.Numerics;
using Robust.Shared.GameStates;

namespace Content.Shared._NC.Mountable.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MountableComponent : Component
{
    /// <summary>
    /// Current Rider
    /// </summary>
    [ViewVariables, AutoNetworkedField]
    public EntityUid? Rider;

    /// <summary>
    /// The displacement of the rider relative to mount
    /// </summary>
    [DataField("riderOffset")]
    public Vector2 RiderOffset = new(0f, -0.4f);

    /// <summary>
    /// Directions of displacement. Useless, but let them be
    /// </summary>
    [DataField("directionOffsets")]
    public Dictionary<Direction, Vector2> DirectionOffsets = new()
    {
        [Direction.North] = new(0f, -0.1f),
        [Direction.South] = new(0f, 0.2f),
        [Direction.East] = Vector2.Zero,
        [Direction.West] = Vector2.Zero,
        [Direction.NorthEast] = new(0f, -0.05f),
        [Direction.NorthWest] = new(0f, -0.05f),
        [Direction.SouthEast] = new(0f, 0.1f),
        [Direction.SouthWest] = new(0f, 0.1f)
    };

    /// <summary>
    /// Speed in the presence of a rider
    /// </summary>
    [DataField("mountedSpeed")]
    public float MountedSpeed = 1.5f;

    /// <summary>
    /// Basic speed returns in the opposite direction
    /// </summary>
    [DataField("defaultSpeed")]
    public float DefaultSpeed = 1.5f;

    /// <summary>
    /// Motion capability switch on mount
    /// </summary>
    [DataField("controlMovement")]
    public bool ControlMovement = true;
}
