using Content.Shared.Store;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._NC.Trade;

[Serializable, NetSerializable,]
public sealed class StoreBuyListingMessage(ProtoId<ListingPrototype> listing, EntityUid actor)
    : BoundUserInterfaceMessage
{
    public ProtoId<ListingPrototype> Listing { get; } = listing;
    public new EntityUid Actor { get; } = actor;
}
