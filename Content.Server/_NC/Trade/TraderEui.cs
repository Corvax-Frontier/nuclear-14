using System.Linq;
using Content.Server.Popups;
using Content.Shared._NC.Trade;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server._NC.Trade;

public sealed class NcStoreSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entMan = null!;
    [Dependency] private readonly TransformSystem _transform = null!;
    [Dependency] private readonly IEntitySystemManager _sysMan = null!;
    [Dependency] private readonly UserInterfaceSystem _ui = null!;
    [Dependency] private readonly IPrototypeManager _prototypes = null!;
    [Dependency] private readonly SharedAudioSystem _audio = null!;
    [Dependency] private readonly PopupSystem _popup = null!;

    private static readonly ISawmill Sawmill = Logger.GetSawmill("ncstore");

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NcStoreComponent, StoreBuyListingBoundUiMessage>(OnBuyRequest);
        SubscribeLocalEvent<CurrencyItemComponent, ComponentStartup>(OnCurrencyAdded);
        SubscribeLocalEvent<CurrencyItemComponent, ComponentShutdown>(OnCurrencyRemoved);
        SubscribeLocalEvent<PlayerDetachedEvent>(OnPlayerMove);
    }

    private void OnCurrencyAdded(Entity<CurrencyItemComponent> ent, ref ComponentStartup args)
        => UpdateNearbyStoreUis(ent.Owner);

    private void OnCurrencyRemoved(Entity<CurrencyItemComponent> ent, ref ComponentShutdown args)
        => UpdateNearbyStoreUis(ent.Owner);

    private void UpdateNearbyStoreUis(EntityUid currencyEntity)
    {
        if (!_entMan.TryGetComponent(currencyEntity, out TransformComponent? xform))
            return;

        var sessions = Filter.Pvs(xform.Coordinates, 3f, _entMan);

        foreach (var session in sessions.Recipients)
        {
            var player = session.AttachedEntity;
            if (player == null || !_entMan.EntityExists(player.Value))
                continue;

            foreach (var (storeUid, storeComp) in EntityQuery<NcStoreComponent>())
            {
                if (!_entMan.TryGetComponent(storeUid, out TransformComponent? storeXform))
                    continue;

                if (!_transform.InRange(storeXform.Coordinates, xform.Coordinates, 3f))
                    continue;

                if (!_ui.IsUiOpen(storeUid, StoreUiKey.Key, player.Value))
                    continue;

                var logic = _sysMan.GetEntitySystem<NcStoreLogicSystem>();
                var balance = logic.GetBalance(player.Value, storeComp.CurrencyWhitelist.FirstOrDefault() ?? "CapCoin");

                var listings = storeComp.Listings
                    .Select(l => new StoreListingData(
                        l.ID,
                        l.Name ?? string.Empty,
                        l.Description ?? string.Empty,
                        l.Icon ?? new SpriteSpecifier.Texture(new("/Textures/Interface/Nano/item-default.png")),
                        (int) l.Cost.First().Value,
                        l.Categories.FirstOrDefault() ?? "Все",
                        StoreMode.Buy,
                        storeComp.CurrencyWhitelist.FirstOrDefault() ?? "CapCoin"
                    ))
                    .ToList();

                _ui.SetUiState(storeUid, StoreUiKey.Key, new StoreUiState(balance, listings));
            }
        }
    }

    private void OnBuyRequest(EntityUid uid, NcStoreComponent comp, StoreBuyListingBoundUiMessage msg)
    {
        var actor = msg.Actor;
        if (!_entMan.EntityExists(actor))
        {
            Sawmill.Warning($"[Buy] Actor entity {actor} does not exist.");
            return;
        }

        if (!_entMan.TryGetComponent(uid, out TransformComponent? storeXform) ||
            !_entMan.TryGetComponent(actor, out TransformComponent? userXform))
        {
            Sawmill.Warning($"[Buy] Missing TransformComponent (store or actor).");
            return;
        }

        if (!_transform.InRange(storeXform.Coordinates, userXform.Coordinates, 3f))
        {
            _popup.PopupEntity(Loc.GetString("Вы слишком далеко от автомата."), uid, actor, PopupType.Medium);
            Sawmill.Warning($"[Buy] User too far from store: {ToPrettyString(actor)} -> {ToPrettyString(uid)}.");
            return;
        }

        var logic = _sysMan.GetEntitySystem<NcStoreLogicSystem>();
        var success = logic.TryPurchase(msg.ListingId, uid, comp, actor);

        if (success)
        {
            _popup.PopupEntity(Loc.GetString("Покупка успешна!"), uid, actor, PopupType.Medium);
            if (comp.ConfirmSound != null)
                _audio.PlayPvs(comp.ConfirmSound, uid);

            Sawmill.Info($"[Buy] {ToPrettyString(actor)} купил '{msg.ListingId}' у {ToPrettyString(uid)}.");
        }
        else
        {
            _popup.PopupEntity(Loc.GetString("Недостаточно средств или товара!"), uid, actor, PopupType.Medium);
            Sawmill.Warning($"[Buy] Покупка не удалась: listingId={msg.ListingId}, user={ToPrettyString(actor)}.");
        }

        var uiSys = _sysMan.GetEntitySystem<StoreStructuredSystem>();
        uiSys.UpdateUiState(uid, comp, actor);
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
}
