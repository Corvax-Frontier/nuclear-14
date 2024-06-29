using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._NC14.DayNightCycle
{
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    public sealed partial class DayNightCycleComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("cycleDuration")]
        public float CycleDurationMinutes { get; set; } = 60f; // Default cycle duration is 60 minutes

        [DataField("timeEntries")]
        public List<TimeEntry> TimeEntries { get; set; } = new()
        {
            new() { Time = 0.00f, ColorHex = "#010105" }, // Midnight
            new() { Time = 0.04f, ColorHex = "#0D1926" }, // Very early morning
            new() { Time = 0.08f, ColorHex = "#1A2E3D" }, // Early dawn
            new() { Time = 0.17f, ColorHex = "#5D492A" }, // Dawn
            new() { Time = 0.25f, ColorHex = "#744A3E" }, // Sunrise
            new() { Time = 0.33f, ColorHex = "#9D7B4D" }, // Early morning
            new() { Time = 0.42f, ColorHex = "#B2904E" }, // Mid-morning
            new() { Time = 0.50f, ColorHex = "#D9C5B6" }, // Noon
            new() { Time = 0.58f, ColorHex = "#C1A78A" }, // Early afternoon
            new() { Time = 0.67f, ColorHex = "#A98E6F" }, // Late afternoon
            new() { Time = 0.75f, ColorHex = "#8C6F4E" }, // Sunset
            new() { Time = 0.83f, ColorHex = "#6E4F3A" }, // Dusk
            new() { Time = 0.92f, ColorHex = "#3A2D2A" }, // Early night
            new() { Time = 1.00f, ColorHex = "#010105" }  // Back to Midnight
        };

        [ViewVariables(VVAccess.ReadWrite)]
        [AutoNetworkedField]
        public int CurrentTimeEntryIndex { get; set; }

        [ViewVariables(VVAccess.ReadWrite)]
        [AutoNetworkedField]
        public float CurrentCycleTime { get; set; }
    }

    [DataDefinition, NetSerializable, Serializable]
    public sealed partial class TimeEntry
    {
        [DataField("colorHex")]
        public string ColorHex { get; set; } = "#FFFFFF";

        [DataField("time")]
        public float Time { get; set; } // Normalized time (0-1)
    }
}
