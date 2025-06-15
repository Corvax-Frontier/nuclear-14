// using Robust.Shared.Serialization;

// namespace Content.Shared._NC.DoorLock;

// [Serializable, NetSerializable]
// public sealed class DoorLockBoundUIState : BoundUserInterfaceState
// {
//     public bool IsLocked { get; }

//     public DoorLockBoundUIState(bool isLocked)
//     {
//         IsLocked = isLocked;
//     }
// }

// [Serializable, NetSerializable]
// public sealed class DoorLockSetCodeMessage : BoundUserInterfaceMessage
// {
//     public string Code { get; }

//     public DoorLockSetCodeMessage(string code)
//     {
//         Code = code;
//     }
// }

// [Serializable, NetSerializable]
// public sealed class DoorLockToggleMessage : BoundUserInterfaceMessage
// {
//     public bool Open { get; }

//     public DoorLockToggleMessage(bool open)
//     {
//         Open = open;
//     }
// }

// [Serializable, NetSerializable]
// public enum DoorLockUiKey : byte
// {
//     Key
// }
