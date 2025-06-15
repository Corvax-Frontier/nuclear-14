// using Content.Shared.Doors.Components;
// using Content.Shared.Interaction.Events;
// using Robust.Shared.Serialization;

// namespace Content.Shared._NC.DoorLock;

// public sealed class DoorLockSystem : EntitySystem
// {
//     [Dependency] private readonly SharedUserInterfaceSystem _ui = default!;
//     public override void Initialize()
//     {
//         base.Initialize();

//         SubscribeLocalEvent<DoorLockComponent, DoorLockToggleMessage>(OnLockUnlock);
//     }

//     private void UpdateDoorLockUi(Entity<DoorLockComponent> door)
//     {
//         var doorComp = CompOrNull<DoorComponent>(door);

//         var isOpen = doorComp?.ClickOpen ?? false;
//         var state = new DoorLockBoundUIState(isOpen);
//         if (TryComp<UserInterfaceComponent>(door, out var uiComp))
//             _ui.SetUiState((door.Owner, uiComp), DoorLockUiKey.Key, state);
//     }

//     private void OnLockUnlock(Entity<DoorLockComponent> ent, ref DoorLockToggleMessage args)
//     {
//         if (!TryComp(ent, out DoorComponent? door))
//             return;

//         if (ent.Comp.Code == 0)
//             return;

//         door.BumpOpen = false;
//         door.CanPry = false;
//         door.ClickOpen = args.Open;
//         Dirty(ent.Owner, door);
//     }
// }
