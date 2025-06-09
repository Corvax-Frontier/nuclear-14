using Content.Shared._NC.Trade;
using Robust.Client.Player;

namespace Content.Client._NC.Trade;

public sealed class StoreStructuredBoundUi : BoundUserInterface
{
    private readonly IPlayerManager _playerManager;
    private StoreStructuredMenu? _menu;

    public StoreStructuredBoundUi(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        IoCManager.InjectDependencies(this);
        _playerManager = IoCManager.Resolve<IPlayerManager>();
    }

    protected override void Open()
    {
        base.Open();
        Logger.Debug("[NcStoreUI] UI открылся (Open()) вызван");

        _menu?.Close(); // гарантированно закрыть предыдущее
        _menu = new StoreStructuredMenu();
        _menu.OnClose += Close;
        _menu.OpenCentered();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;

        Logger.Debug("[NcStoreUI] Dispose: окно уничтожено");
        _menu = null;
    }

    public void SetupFromState(StoreUiState state)
    {
        Logger.Debug($"[NcStoreUI] SetupFromState вызван. Состояние получено: {state.Listings.Count} товаров, Баланс: {state.Balance}");
        _menu?.Setup(state.Listings, state.Balance, OnBuy);
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is StoreUiState storeState)
        {
            Logger.Debug($"[NcStoreUI] UpdateState вызван: Баланс = {storeState.Balance}, товаров = {storeState.Listings.Count}");
            SetupFromState(storeState);
        }
        else
        {
            Logger.Warning("[NcStoreUI] Получено неизвестное состояние UI");
        }
    }

    private void OnBuy(string listingId)
    {
        Logger.Debug($"[NcStoreUI] Покупка товара: {listingId}");
        SendMessage(new StoreBuyListingBoundUiMessage(listingId));
    }
}
