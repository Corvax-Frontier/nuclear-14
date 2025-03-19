using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._NC.ResourceGatheringSystem;

[Serializable, NetSerializable]
public sealed partial class GatherResourceDoAfterEvent : SimpleDoAfterEvent
{
}
