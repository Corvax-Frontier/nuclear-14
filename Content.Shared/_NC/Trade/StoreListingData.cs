using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._NC.Trade;

[Serializable, NetSerializable,]
public sealed record StoreListingData(
    string Id,
    string Name,
    string Description,
    SpriteSpecifier Icon,
    int Price,
    string Category,
    StoreMode CategoryMode
);
