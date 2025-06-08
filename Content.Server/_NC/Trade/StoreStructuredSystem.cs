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

    public override void Initialize() => SubscribeLocalEvent<StoreComponent, ActivateInWorldEvent>(OnActivate);

    private void OnActivate(EntityUid uid, StoreComponent comp, ActivateInWorldEvent args)
    {
        if (!_ui.HasUi(uid, StoreUiKey.Key))
            return;

        if (!_ui.IsUiOpen(uid, StoreUiKey.Key, args.User))
            _ui.OpenUi(uid, StoreUiKey.Key, args.User);

        UpdateUiState(uid, comp, args.User);
    }

    /// <summary>
    /// Обновляет интерфейс магазина: баланс и список доступных товаров.
    /// </summary>
    private void UpdateUiState(EntityUid uid, StoreComponent comp, EntityUid user)
    {
        var currency = comp.CurrencyWhitelist.FirstOrDefault() ?? string.Empty;
        var balance = currency != string.Empty ? _logic.GetBalance(user, currency) : 0;

        var data = comp.Listings
            .Where(x => x.Name != null)
            .Select(
                x =>
            {
                var cost = x.Cost.FirstOrDefault().Value;

                return new StoreListingData(
                    x.ID,
                    x.Name!,
                    x.Description ?? string.Empty,
                    x.Icon ?? new SpriteSpecifier.Texture(new("/Textures/Interface/Nano/item-default.png")),
                    (int)cost,
                    x.Categories.FirstOrDefault() ?? "Разное",
                    cost < 0 ? StoreMode.Sell : StoreMode.Buy
                );
            })
            .ToList();

        _ui.SetUiState(uid, StoreUiKey.Key, new StoreUiState(balance, data));
    }
}
