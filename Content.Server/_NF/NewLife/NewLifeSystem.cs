using Content.Server.GameTicking;
using Content.Shared._NF.NewLife;
using Content.Shared.GameTicking;
using Content.Shared.Preferences;
using Robust.Server.Player;
using Robust.Shared.Enums;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Server._NF.NewLife;

public sealed class NewLifeSystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _player = default!;

    private readonly Dictionary<ICommonSession, HashSet<HumanoidCharacterProfile>> _usedProfiles = [];

    public override void Initialize()
    {
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayerSpawnComplete);
        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundRestartCleanup);

        _player.PlayerStatusChanged += OnPlayerStatusChanged;
    }

    private void OnPlayerSpawnComplete(PlayerSpawnCompleteEvent e)
    {
        _usedProfiles.GetOrNew(e.Player).Add(e.Profile);

        SendUsedProfiles(e.Player);
    }

    private void OnRoundRestartCleanup(RoundRestartCleanupEvent e)
    {
        HashSet<HumanoidCharacterProfile> empty = [];

        foreach (var session in _usedProfiles.Keys)
            RaiseNetworkEvent(new UsedProfilesEvent(empty), session);

        _usedProfiles.Clear();
    }

    private void OnPlayerStatusChanged(object? sender, SessionStatusEventArgs e)
    {
        if (e.NewStatus == SessionStatus.Connected)
            SendUsedProfiles(e.Session);
    }

    private void SendUsedProfiles(ICommonSession session)
    {
        if (!_usedProfiles.TryGetValue(session, out var usedProfiles))
            return;

        RaiseNetworkEvent(new UsedProfilesEvent(usedProfiles), session);
    }

    public bool IsProfileUsed(ICommonSession player, HumanoidCharacterProfile profile)
    {
        if (!_usedProfiles.TryGetValue(player, out var usedProfiles))
            return false;

        return usedProfiles.Contains(profile);
    }
}
