using Robust.Shared.Serialization;


namespace Content.Shared._NC.Trade;

[Serializable, NetSerializable,]
public sealed class StoreUiState(int balance, List<StoreListingData> listings) : BoundUserInterfaceState
{
    public readonly int Balance = balance;
    public readonly List<StoreListingData> Listings = listings;
}
