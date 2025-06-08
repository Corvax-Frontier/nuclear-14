using Robust.Shared.GameStates;

namespace Content.Shared._NC.Trade;

[RegisterComponent, NetworkedComponent,]
public sealed partial class CurrencyItemComponent : Component
{
    [DataField("currency")]
    public string Currency = string.Empty;

    [DataField("amount")]
    public int Amount = 1;
}
