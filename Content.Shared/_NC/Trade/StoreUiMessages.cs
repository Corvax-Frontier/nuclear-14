using Robust.Shared.Serialization;

namespace Content.Shared._NC.Trade;

/// <summary>
/// Сообщение клиента на сервер с запросом покупки.
/// </summary>
[Serializable, NetSerializable]
public sealed class StoreBuyListingBoundUiMessage : BoundUserInterfaceMessage
{
    public string ListingId;

    public StoreBuyListingBoundUiMessage(string listingId)
    {
        ListingId = listingId;
    }
}
