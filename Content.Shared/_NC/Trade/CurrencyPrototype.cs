using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._NC.Trade;

[Prototype("currency")]
public sealed class CurrencyPrototype : IPrototype
{
    [IdDataField] public string ID { get; private set; } = string.Empty;

    [DataField("name")]
    public string Name { get; private set; } = string.Empty;

    [DataField("icon")]
    public SpriteSpecifier Icon { get; private set; } = new SpriteSpecifier.Texture(new("/Textures/Interface/Nano/currency-default.png"));

    [DataField("entity")]
    public string Entity { get; private set; } = null!;
}
