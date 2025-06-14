using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._NC.Trade;

/// <summary>
/// Прототип товара в магазине (используется в StoreComponent)
/// </summary>
[Prototype("storeListing")]
public sealed class StoreListingPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; set; } = string.Empty;

    [DataField("name")]
    public string? Name;

    [DataField("description")]
    public string? Description;

    [DataField("icon")]
    public SpriteSpecifier? Icon;

    [DataField("productEntity")]
    public string? ProductEntity;

    [DataField("cost")]
    public Dictionary<string, FixedPoint2> Cost = new();

    [DataField("categories")]
    public List<string> Categories = new();

    [DataField("conditions")]
    public List<ListingConditionPrototype> Conditions = new();
}
