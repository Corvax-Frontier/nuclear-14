using Content.Shared._NC.Trade;
using StoreBuyListingMessage = Content.Shared._NC.Trade.StoreBuyListingMessage;

namespace Content.Server._NC.Trade;

public sealed class StoreStructuredUiSystem : EntitySystem
{
    [Dependency] private readonly NcStoreLogicSystem _logic = null!;

    public override void Initialize() =>
        SubscribeLocalEvent<StoreComponent, StoreBuyListingMessage>(OnBuyListing);

    private void OnBuyListing(EntityUid uid, StoreComponent comp, StoreBuyListingMessage msg)
    {
        var user = msg.Actor;

        if (!Exists(user))
            return;

        _logic.TryPurchase(msg.Listing, uid, comp, user);
    }
}
