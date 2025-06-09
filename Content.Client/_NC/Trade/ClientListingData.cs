using Content.Shared._NC.Trade;
using Robust.Shared.Utility;

namespace Content.Client._NC.Trade;

/// <summary>
/// Клиентская копия информации о товаре.
/// </summary>
public sealed class ClientListingData
{
    public string Id = string.Empty;
    public string Name = string.Empty;
    public string Description = string.Empty;
    public SpriteSpecifier Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/Nano/item-default.png"));
    public int Price;
    public string Category = "Разное";
    public StoreMode CategoryMode = StoreMode.Buy;
    public string CurrencyId = "CapCoin"; // не readonly
}
