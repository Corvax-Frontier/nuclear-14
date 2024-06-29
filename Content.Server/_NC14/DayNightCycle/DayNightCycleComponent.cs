using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using System.Collections.Generic;

namespace Content.Server._NC14.DayNightCycle
{
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    public sealed partial class DayNightCycleComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("cycleDuration")]
        public float CycleDurationMinutes { get; set; } = 60f; // Default cycle duration is 60 minutes

        [DataField("timeEntries")]
        public List<TimeEntry> TimeEntries { get; set; } = new();

        [ViewVariables(VVAccess.ReadWrite)]
        [AutoNetworkedField]
        public int CurrentTimeEntryIndex { get; set; }

        [ViewVariables(VVAccess.ReadWrite)]
        [AutoNetworkedField]
        public float CurrentCycleTime { get; set; }
    }

    [DataDefinition, NetSerializable]
    public sealed partial class TimeEntry
    {
        [DataField("colorHex")]
        public string ColorHex { get; set; } = "#FFFFFF";

        [DataField("time")]
        public float Time { get; set; } // Normalized time (0-1)
    }
}
