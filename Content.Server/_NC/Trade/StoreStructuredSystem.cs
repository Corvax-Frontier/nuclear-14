using Content.Shared._NC.Trade;
using Content.Shared.Interaction;
using Robust.Server.GameObjects;
using Robust.Shared.Utility;
using System.Linq;
using StoreUiKey = Content.Shared._NC.Trade.StoreUiKey;

namespace Content.Server._NC.Trade;

public sealed class StoreStructuredSystem : EntitySystem
{
    [Dependency] private readonly UserInterfaceSystem _ui = null!;
    [Dependency] private readonly NcStoreLogicSystem _logic = null!;

    public override void Initialize()
    {
        SubscribeLocalEvent<NcStoreComponent, ActivateInWorldEvent>(OnActivate);
    }

    private void OnActivate(EntityUid uid, NcStoreComponent comp, ActivateInWorldEvent args)
    {
        if (!_ui.HasUi(uid, StoreUiKey.Key))
        {
            Logger.Warning($"[NcStoreServer] Нет интерфейса StoreUiKey.Key на {ToPrettyString(uid)}.");
            return;
        }

        Logger.Debug($"[NcStoreServer] Попытка открыть UI {StoreUiKey.Key} для {ToPrettyString(args.User)}");

        // Если уже открыт — закрыть
        if (_ui.IsUiOpen(uid, StoreUiKey.Key, args.User))
        {
            _ui.CloseUi(uid, StoreUiKey.Key, args.User);
        }

        // Затем открыть заново
        _ui.OpenUi(uid, StoreUiKey.Key, args.User);
        UpdateUiState(uid, comp, args.User);
    }


    public void UpdateUiState(EntityUid uid, NcStoreComponent comp, EntityUid user)
    {
        if (comp.CurrencyWhitelist.Count == 0)
        {
            Logger.Warning($"[NcStore] Магазин {ToPrettyString(uid)} не имеет валюты.");
            return;
        }

        var currency = comp.CurrencyWhitelist.First();
        var balance = _logic.GetBalance(user, currency);

        var data = comp.Listings
            .Where(x => x.Name != null)
            .Select(x =>
            {
                var cost = x.Cost.FirstOrDefault().Value;

                return new StoreListingData(
                    x.ID,
                    x.Name!,
                    x.Description ?? string.Empty,
                    x.Icon ?? new SpriteSpecifier.Rsi(new("/Textures/_Nuclear14/Objects/Misc/currency.rsi"), "caps"),
                    (int)cost,
                    x.Categories.FirstOrDefault() ?? "Разное",
                    cost < 0 ? StoreMode.Sell : StoreMode.Buy,
                    currency
                );
            })
            .ToList();

        Logger.Debug($"[NcStoreServer] UI для {ToPrettyString(user)}: {data.Count} товаров, Валюта: {currency}, Баланс: {balance}");

        _ui.SetUiState(uid, StoreUiKey.Key, new StoreUiState(balance, data));
    }

}
