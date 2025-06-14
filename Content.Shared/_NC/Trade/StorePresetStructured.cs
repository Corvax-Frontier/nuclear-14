using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;
namespace Content.Shared._NC.Trade;

[Prototype("storePresetStructured")]
public sealed class StorePresetStructuredPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; set; } = null!;

    [DataField("currency", customTypeSerializer: typeof(PrototypeIdSerializer<NcCurrencyPrototype>))]
    public string Currency = "CapCoin";

    [DataField("catalog")]
    public Dictionary<string, Dictionary<string, List<StructuredListing>>> Catalog = new();
}

[DataDefinition]
public sealed partial class StructuredListing
{
    [DataField("proto", required: true)]
    public string Proto = null!;

    [DataField("price")]
    public int Price;

    [DataField("name")]
    public string? Name;

    [DataField("description")]
    public string? Description;

    [DataField("icon")]
    public SpriteSpecifier? Icon;
}
