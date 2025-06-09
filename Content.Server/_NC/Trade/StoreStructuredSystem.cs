using System.Linq;
using Content.Shared._NC.Trade;
using Content.Shared.Interaction;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Server._NC.Trade;

public sealed class StoreStructuredSystem : EntitySystem
{
    [Dependency] private readonly UserInterfaceSystem _ui = null!;
    [Dependency] private readonly NcStoreLogicSystem _logic = null!;
    [Dependency] private readonly TransformSystem _transform = null!;
    [Dependency] private readonly IEntityManager _entMan = null!;

    public override void Initialize()
    {
        SubscribeLocalEvent<NcStoreComponent, ActivateInWorldEvent>(OnActivate);
        SubscribeLocalEvent<PlayerDetachedEvent>(OnPlayerMove);
    }

    private void OnActivate(EntityUid uid, NcStoreComponent comp, ActivateInWorldEvent args)
    {
        // Закрыть все другие UI, если открыты
        foreach (var (otherUid, _) in EntityQuery<NcStoreComponent>())
        {
            if (otherUid == uid)
                continue;

            if (_ui.IsUiOpen(otherUid, StoreUiKey.Key, args.User))
                _ui.CloseUi(otherUid, StoreUiKey.Key, args.User);
        }

        // Закрыть текущее окно, если уже открыто
        if (_ui.IsUiOpen(uid, StoreUiKey.Key, args.User))
        {
            _ui.CloseUi(uid, StoreUiKey.Key, args.User);
            return;
        }

        _ui.OpenUi(uid, StoreUiKey.Key, args.User);
        UpdateUiState(uid, comp, args.User);
    }

    private void OnPlayerMove(PlayerDetachedEvent args)
    {
        var player = args.Player.AttachedEntity;
        if (player == null || !_entMan.EntityExists(player.Value))
            return;

        foreach (var (uid, comp) in EntityQuery<NcStoreComponent>())
        {
            if (!_ui.IsUiOpen(uid, StoreUiKey.Key, player.Value))
                continue;

            if (!_entMan.TryGetComponent(uid, out TransformComponent? storeXform) ||
                !_entMan.TryGetComponent(player.Value, out TransformComponent? userXform))
                continue;

            if (!_transform.InRange(storeXform.Coordinates, userXform.Coordinates, 3f))
            {
                _ui.CloseUi(uid, StoreUiKey.Key, player.Value);
            }
        }
    }

    public void UpdateUiState(EntityUid uid, NcStoreComponent comp, EntityUid actor)
    {
        if (comp.CurrencyWhitelist.Count == 0)
        {
            Logger.Warning($"[NcStore] Магазин {ToPrettyString(uid)} не имеет валюты.");
            return;
        }

        var currency = comp.CurrencyWhitelist.First();

        // Получаем всех игроков через IPlayerManager
        var playerManager = IoCManager.Resolve<IPlayerManager>();

        foreach (var session in playerManager.Sessions)
        {
            var player = session.AttachedEntity;
            if (player == null || !_entMan.EntityExists(player.Value))
                continue;

            if (!_ui.IsUiOpen(uid, StoreUiKey.Key, player.Value))
                continue;

            var balance = _logic.GetBalance(player.Value, currency);

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

            _ui.SetUiState(uid, StoreUiKey.Key, new StoreUiState(balance, data));
        }
    }
}
