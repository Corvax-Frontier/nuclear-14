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

        _menu = new StoreStructuredMenu();
        _menu.OnClose += Close;
        _menu.OpenCentered();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;

        _menu = null;
    }

    public void SetupFromState(StoreUiState state) => _menu?.Setup(state.Listings, state.Balance, OnBuy);

    private void OnBuy(string listingId)
    {
        SendMessage(new StoreBuyListingBoundUiMessage(listingId));
    }
}
