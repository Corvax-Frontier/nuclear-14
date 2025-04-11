using Robust.Shared.GameStates;

namespace Content.Shared._Shitmed.Medical.Surgery.Tools;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BoneSawComponent : Component, ISurgeryToolComponent
{
    public string ToolName => "Медицинская пила"; // Corvax-Localization
    public bool? Used { get; set; } = null;
    [DataField, AutoNetworkedField]
    public float Speed { get; set; } = 1f;
}
