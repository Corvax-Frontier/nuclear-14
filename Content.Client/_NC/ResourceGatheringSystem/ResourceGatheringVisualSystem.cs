using Robust.Client.Audio;
using Robust.Client.Player;
using Robust.Shared.Audio;

namespace Content.Client._NC.ResourceGatheringSystem;

public sealed class ResourceGatheringVisualSystem : EntitySystem
{
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    private static readonly ISawmill Sawmill = Logger.GetSawmill("ResourceGatheringVisual");

    public void ShowGatherPopup(EntityUid user, string text)
    {
        var session = _playerManager.LocalSession;
        if (session?.AttachedEntity != user)
            return;

        Sawmill.Info($"[GatheringVisual] Popup: {text}");
    }

    public void PlayGatherAnimation(EntityUid user, EntityUid target)
    {
        var session = _playerManager.LocalSession;
        if (session?.AttachedEntity != user)
            return;

        Sawmill.Info($"[GatheringVisual] {user} анимирует добычу ресурса {target}");
    }

    public void ShowProgressBar(EntityUid user, float duration, EntityUid target)
    {
        var session = _playerManager.LocalSession;
        if (session?.AttachedEntity != user)
            return;

        Sawmill.Info($"[GatheringVisual] Показываем прогресс-бар на {duration} сек");
    }

    public void PlaySound(EntityUid user, string soundPath)
    {
        var session = _playerManager.LocalSession;
        if (session?.AttachedEntity != user)
            return;

        _audio.PlayGlobal(
            soundPath,
            user,
            new AudioParams
            {
                Volume = 1f
            }
        );
    }
}
