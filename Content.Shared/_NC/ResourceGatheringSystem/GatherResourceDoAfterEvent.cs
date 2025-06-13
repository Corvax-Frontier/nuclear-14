using Robust.Shared.Serialization;
using Content.Shared.DoAfter;

namespace Content.Shared._NC.ResourceGatheringSystem;

[Serializable, NetSerializable]
public sealed partial class GatherResourceDoAfterEvent : SimpleDoAfterEvent
{
}
