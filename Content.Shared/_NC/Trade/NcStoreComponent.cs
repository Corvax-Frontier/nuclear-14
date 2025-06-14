using Robust.Shared.Audio;


namespace Content.Shared._NC.Trade;

[RegisterComponent]
public sealed partial class NcStoreComponent : Component
{
    [DataField("preset")]
    public string Preset = string.Empty;

    // Валюта, которая будет использоваться в этом магазине
    [DataField("currencyWhitelist")]
    public List<string> CurrencyWhitelist = new();

    // Категории магазина (например, "Оружие", "Мусор", "Инструменты")
    [DataField("categories")]
    public List<string> Categories = new();

    // Список товаров, которые может продать/купить этот магазин
    [DataField("listings")]
    public List<StoreListingPrototype> Listings = new();

    [DataField("confirmSound")]
    public SoundSpecifier? ConfirmSound = new SoundPathSpecifier("/Audio/Effects/Cargo/ping.ogg");
    /// <summary>
    /// Позволяет использовать деструктуризацию в foreach (var (uid, comp) in ...)
    /// </summary>
    public void Deconstruct(out EntityUid uid, out NcStoreComponent comp)
    {
        uid = Owner;
        comp = this;
    }
}
