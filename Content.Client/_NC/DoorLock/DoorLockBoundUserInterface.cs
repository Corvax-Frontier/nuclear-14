// using Content.Shared._NC.DoorLock;
// using JetBrains.Annotations;

// namespace Content.Client._NC.DoorLock;

// [UsedImplicitly]
// public sealed class DoorLockBoundUserInterface : BoundUserInterface
// {
//     [ViewVariables]
//     private DoorLockWindow? _window;

//     public DoorLockBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
//     {
//     }

//     protected override void Open()
//     {
//         base.Open();

//         _window = new DoorLockWindow();

//         _window.OnClose += Close;
//         _window.OpenClose += enabled => SendMessage(new DoorLockToggleMessage(enabled));
//         _window.OnCodeChanged += code => SendMessage(new DoorLockSetCodeMessage(code));

//         _window.OpenCentered();
//     }

//     protected override void Dispose(bool disposing)
//     {
//         base.Dispose(disposing);
//         if (!disposing)
//             return;
//         _window?.Dispose();
//     }

//     protected override void UpdateState(BoundUserInterfaceState state)
//     {
//         base.UpdateState(state);

//         if (state is not DoorLockBoundUIState msg)
//             return;

//         _window?.UpdateState(msg);
//     }
// }
