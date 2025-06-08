using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._NC.Trade;

/// <summary>
/// Сообщение клиента на сервер с запросом покупки.
/// </summary>
[Serializable, NetSerializable]
public sealed class StoreBuyListingEuiMessage : EuiMessageBase
{
    public string ListingId;

    public StoreBuyListingEuiMessage(string listingId)
    {
        ListingId = listingId;
    }
}
