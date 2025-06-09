# Команды воспроизведения

cmd-replay-play-desc = Возобновить воспроизведение записи.
cmd-replay-play-help = replay_play

cmd-replay-pause-desc = Приостановить воспроизведение записи.
cmd-replay-pause-help = replay_pause

cmd-replay-toggle-desc = Возобновить или приостановить воспроизведение записи.
cmd-replay-toggle-help = replay_toggle

cmd-replay-stop-desc = Остановить и выгрузить запись.
cmd-replay-stop-help = replay_stop

cmd-replay-load-desc = Загрузить и начать воспроизведение записи.
cmd-replay-load-help = replay_load <папка с записью>
cmd-replay-load-hint = Папка с записью

cmd-replay-skip-desc = Перемотать вперед или назад.
cmd-replay-skip-help = replay_skip <тики или временной интервал>
cmd-replay-skip-hint = Тики или временной интервал (ЧЧ:ММ:СС).

cmd-replay-set-time-desc = Перейти к определенному моменту времени.
cmd-replay-set-time-help = replay_set <тик или время>
cmd-replay-set-time-hint = Тик или временной интервал (ЧЧ:ММ:СС), начиная с

cmd-replay-error-time = "{$time}" не является числом или временным интервалом.
cmd-replay-error-args = Неверное количество аргументов.
cmd-replay-error-no-replay = В данный момент запись не воспроизводится.
cmd-replay-error-already-loaded = Запись уже загружена.
cmd-replay-error-run-level = Невозможно загрузить запись при подключении к серверу.

# Команды записи

cmd-replay-recording-start-desc = Начать запись, опционально с ограничением по времени.
cmd-replay-recording-start-help = Использование: replay_recording_start [имя] [перезапись] [лимит времени]
cmd-replay-recording-start-success = Начата запись.
cmd-replay-recording-start-already-recording = Запись уже ведется.
cmd-replay-recording-start-error = Ошибка при запуске записи.
cmd-replay-recording-start-hint-time = [лимит времени (минуты)]
cmd-replay-recording-start-hint-name = [имя]
cmd-replay-recording-start-hint-overwrite = [перезапись (да/нет)]

cmd-replay-recording-stop-desc = Остановить запись.
cmd-replay-recording-stop-help = Использование: replay_recording_stop
cmd-replay-recording-stop-success = Запись остановлена.
cmd-replay-recording-stop-not-recording = В данный момент запись не ведется.

cmd-replay-recording-stats-desc = Показать статистику текущей записи.
cmd-replay-recording-stats-help = Использование: replay_recording_stats
cmd-replay-recording-stats-result = Длительность: {$time} мин, Тиков: {$ticks}, Размер: {$size} МБ, скорость: {$rate} МБ/мин.

# Интерфейс управления временем
replay-time-box-scrubbing-label = Динамическая перемотка
replay-time-box-replay-time-label = Время записи: {$current} / {$end} ({$percentage}%)
replay-time-box-server-time-label = Серверное время: {$current} / {$end}
replay-time-box-index-label = Индекс: {$current} / {$total}
replay-time-box-tick-label = Тик: {$current} / {$total}
