using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Random;

namespace Content.Client._NC.ResourceGatheringSystem;

public sealed class ResourceGatheringVisualSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlayManager = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IAudioSystem _audio = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    public override void Initialize()
    {
        base.Initialize();
        // Можно подписаться на клиентские события или NetworkEvents
    }

    // Локальный PopUp на клиента
    public void ShowGatherPopup(EntityUid user, string text)
    {
        if (_playerManager.LocalPlayer?.ControlledEntity != user)
            return;

        Logger.Info($"[GatheringVisual] Popup: {text}");
        // Тут можно вызвать систему LocalPopup или вывод на экран
    }

    // Простая анимация удара по ресурсу
    public void PlayGatherAnimation(EntityUid user, EntityUid target)
    {
        Logger.Info($"[GatheringVisual] {user} анимирует добычу ресурса {target}");
        // Можно добавить спрайт-флип или Local Animation System
    }

    // Прогресс-бар сбора
    public void ShowProgressBar(EntityUid user, float duration, EntityUid target)
    {
        if (_playerManager.LocalPlayer?.ControlledEntity != user)
            return;

        Logger.Info($"[GatheringVisual] Показываем прогресс-бар на {duration} сек");
        // Можно реализовать кастомный HUD элемент или бар
    }

    // Воспроизведение звука на клиенте
    public void PlaySound(EntityUid user, string soundPath)
    {
        if (_playerManager.LocalPlayer?.ControlledEntity != user)
            return;

        var filter = Filter.Local();
        _audio.Play(soundPath, filter);
    }

    // Пуск эффекта добычи (пыль, искры и т.п.)
    public void PlayEffect(EntityUid target)
    {
        Logger.Info($"[GatheringVisual] Воспроизводим эффект добычи на {target}");
        // Можно подключить Particle System или создать спрайтовый эффект
    }
}
