### Локализация консольных команд движка

## Общие ошибки команд

cmd-invalid-arg-number-error = Некорректное количество аргументов.

cmd-parse-failure-integer = {$arg} не является целым числом.
cmd-parse-failure-float = {$arg} не является числом с плавающей точкой.
cmd-parse-failure-bool = {$arg} не является булевым значением.
cmd-parse-failure-uid = {$arg} не является корректным UID сущности.
cmd-parse-failure-mapid = {$arg} не является корректным MapId.
cmd-parse-failure-enum = {$arg} не является перечислением {$enum}.
cmd-parse-failure-grid = {$arg} не является корректной сеткой.
cmd-parse-failure-entity-exist = UID {$arg} не соответствует существующей сущности.
cmd-parse-failure-session = Нет сессии с именем пользователя: {$username}

cmd-error-file-not-found = Не удалось найти файл: {$file}.
cmd-error-dir-not-found = Не удалось найти директорию: {$dir}.

cmd-failure-no-attached-entity = Нет сущности, привязанной к этой оболочке.

## Команда 'help'
cmd-help-desc = Показать общую справку или справку по конкретной команде
cmd-help-help = Использование: help [имя команды]
    Без указания имени команды отображает общую справку. С указанием имени команды - справку по этой команде.

cmd-help-no-args = Для отображения справки по конкретной команде введите 'help <команда>'. Для списка всех доступных команд введите 'list'. Для поиска команд используйте 'list <фильтр>'.
cmd-help-unknown = Неизвестная команда: { $command }
cmd-help-top = { $command } - { $description }
cmd-help-invalid-args = Некорректное количество аргументов.
cmd-help-arg-cmdname = [имя команды]

## Команда 'cvar'
cmd-cvar-desc = Получить или установить CVar.
cmd-cvar-help = Использование: cvar <имя | ?> [значение]
    Если указано значение, оно будет разобрано и сохранено как новое значение CVar.
    Если нет, будет отображено текущее значение CVar.
    Используйте 'cvar ?' для получения списка всех зарегистрированных CVar.

cmd-cvar-invalid-args = Требуется ровно один или два аргумента.
cmd-cvar-not-registered = CVar '{ $cvar }' не зарегистрирован. Используйте 'cvar ?' для списка всех CVar.
cmd-cvar-parse-error = Входное значение имеет неверный формат для типа { $type }
cmd-cvar-compl-list = Показать доступные CVar
cmd-cvar-arg-name = <имя | ?>
cmd-cvar-value-hidden = <значение скрыто>

## Команда 'cvar_subs'
cmd-cvar_subs-desc = Показать подписки OnValueChanged для CVar.
cmd-cvar_subs-help = Использование: cvar_subs <имя>

cmd-cvar_subs-invalid-args = Требуется ровно один аргумент.
cmd-cvar_subs-arg-name = <имя>

## Команда 'list'
cmd-list-desc = Показать список доступных команд с возможностью фильтрации
cmd-list-help = Использование: list [фильтр]
    Показывает все доступные команды. Если указан аргумент, он будет использоваться для фильтрации команд по имени.

cmd-list-heading = СТОРОНА ИМЯ            ОПИСАНИЕ{"\u000A"}-------------------------{"\u000A"}

cmd-list-arg-filter = [фильтр]

## Команда '>' (удаленное выполнение)
cmd-remoteexec-desc = Выполнить команды на стороне сервера
cmd-remoteexec-help = Использование: > <команда> [арг] [арг] [арг...]
    Выполняет команду на сервере. Необходимо, если команда с таким же именем существует на клиенте.

## Команда 'gc'
cmd-gc-desc = Запустить сборщик мусора (GC)
cmd-gc-help = Использование: gc [поколение]
    Использует GC.Collect() для запуска сборщика мусора.
    Если указан аргумент, он интерпретируется как номер поколения GC.
    Используйте команду 'gfc' для полной компактификации LOH.

cmd-gc-failed-parse = Ошибка разбора аргумента.
cmd-gc-arg-generation = [поколение]

## Команда 'gcf'
cmd-gcf-desc = Полная сборка мусора с компактификацией LOH.
cmd-gcf-help = Использование: gcf
    Выполняет полную сборку GC.Collect(2, GCCollectionMode.Forced, true, true) с компактификацией LOH.
    Может вызвать задержку в сотни миллисекунд.

## Команда 'gc_mode'
cmd-gc_mode-desc = Изменить/показать режим задержки GC
cmd-gc_mode-help = Использование: gc_mode [тип]
    Без аргументов показывает текущий режим.
    С аргументом устанавливает указанный режим GCLatencyMode.

cmd-gc_mode-current = текущий режим GC: { $prevMode }
cmd-gc_mode-possible = возможные режимы:
cmd-gc_mode-option = - { $mode }
cmd-gc_mode-unknown = неизвестный режим GC: { $arg }
cmd-gc_mode-attempt = попытка изменить режим GC: { $prevMode } -> { $mode }
cmd-gc_mode-result = текущий режим GC: { $mode }
cmd-gc_mode-arg-type = [тип]

## Команда 'mem'
cmd-mem-desc = Показать информацию об управляемой памяти
cmd-mem-help = Использование: mem

cmd-mem-report = Размер кучи: { TOSTRING($heapSize, "N0") }
    Всего выделено: { TOSTRING($totalAllocated, "N0") }

## Команда 'physics'
cmd-physics-overlay = {$overlay} - неизвестный оверлей

## Команда 'lsasm'
cmd-lsasm-desc = Показать загруженные сборки по контексту загрузки
cmd-lsasm-help = Использование: lsasm

## Команда 'exec'
cmd-exec-desc = Выполнить скрипт из пользовательских данных игры
cmd-exec-help = Использование: exec <имя файла>
    Каждая строка файла выполняется как команда, кроме начинающихся с #

cmd-exec-arg-filename = <имя файла>

## Команда 'dump_net_comps'
cmd-dump_net_comps-desc = Показать таблицу сетевых компонентов.
cmd-dump_net_comps-help = Использование: dump_net-comps

cmd-dump_net_comps-error-writeable = Регистрация еще доступна для записи, сетевые ID не сгенерированы.
cmd-dump_net_comps-header = Регистрации сетевых компонентов:

## Команда 'dump_event_tables'
cmd-dump_event_tables-desc = Показать таблицы направленных событий для сущности.
cmd-dump_event_tables-help = Использование: dump_event_tables <entityUid>

cmd-dump_event_tables-missing-arg-entity = Отсутствует аргумент сущности
cmd-dump_event_tables-error-entity = Некорректная сущность
cmd-dump_event_tables-arg-entity = <entityUid>

## Команда 'monitor'
cmd-monitor-desc = Переключить монитор отладки в меню F3.
cmd-monitor-help = Использование: monitor <имя>
    Доступные мониторы: { $monitors }
    Можно использовать "-all" и "+all" для скрытия/показа всех мониторов.

cmd-monitor-arg-monitor = <монитор>
cmd-monitor-invalid-name = Некорректное имя монитора
cmd-monitor-arg-count = Отсутствует аргумент монитора
cmd-monitor-minus-all-hint = Скрыть все мониторы
cmd-monitor-plus-all-hint = Показать все мониторы

## Команда 'setambientlight'
cmd-set-ambient-light-desc = Установить фоновое освещение для указанной карты в SRGB.
cmd-set-ambient-light-help = setambientlight [mapid] [r g b a]
cmd-set-ambient-light-parse = Не удалось разобрать аргументы как байты цвета.

## Команды работы с картами

cmd-savemap-desc = Сохранить карту на диск. Не сохраняет пост-инициализированные карты без принуждения.
cmd-savemap-help = savemap <MapID> <Путь> [force]
cmd-savemap-not-exist = Целевая карта не существует.
cmd-savemap-init-warning = Попытка сохранить пост-инициализированную карту без принуждения.
cmd-savemap-attempt = Попытка сохранить карту {$mapId} в {$path}.
cmd-savemap-success = Карта успешно сохранена.
cmd-hint-savemap-id = <MapID>
cmd-hint-savemap-path = <Путь>
cmd-hint-savemap-force = [bool]

cmd-loadmap-desc = Загрузить карту с диска в игру.
cmd-loadmap-help = loadmap <MapID> <Путь> [x] [y] [rotation] [consistentUids]
cmd-loadmap-nullspace = Нельзя загрузить в карту 0.
cmd-loadmap-exists = Карта {$mapId} уже существует.
cmd-loadmap-success = Карта {$mapId} загружена из {$path}.
cmd-loadmap-error = Ошибка при загрузке карты из {$path}.
cmd-hint-loadmap-x-position = [x-позиция]
cmd-hint-loadmap-y-position = [y-позиция]
cmd-hint-loadmap-rotation = [вращение]
cmd-hint-loadmap-uids = [float]

cmd-hint-savebp-id = <Grid EntityID>

## Команда 'flushcookies'
cmd-flushcookies-desc = Сохранить куки CEF на диск
cmd-flushcookies-help = Обеспечивает сохранение кук при аварийном завершении.
    Операция выполняется асинхронно.

cmd-ldrsc-desc = Предварительно загрузить ресурс.
cmd-ldrsc-help = Использование: ldrsc <путь> <тип>

cmd-rldrsc-desc = Перезагрузить ресурс.
cmd-rldrsc-help = Использование: rldrsc <путь> <тип>

cmd-gridtc-desc = Получить количество тайлов сетки.
cmd-gridtc-help = Использование: gridtc <gridId>

# Клиентские команды
cmd-guidump-desc = Сохранить дерево GUI в /guidump.txt.
cmd-guidump-help = Использование: guidump

cmd-uitest-desc = Открыть тестовое окно UI
cmd-uitest-help = Использование: uitest

## Команда 'uitest2'
cmd-uitest2-desc = Открыть окно тестирования элементов управления UI
cmd-uitest2-help = Использование: uitest2 <вкладка>
cmd-uitest2-arg-tab = <вкладка>
cmd-uitest2-error-args = Ожидается не более одного аргумента
cmd-uitest2-error-tab = Некорректная вкладка: '{$value}'
cmd-uitest2-title = UITest2

cmd-setclipboard-desc = Установить текст в буфер обмена
cmd-setclipboard-help = Использование: setclipboard <текст>

cmd-getclipboard-desc = Получить текст из буфера обмена
cmd-getclipboard-help = Использование: Getclipboard

cmd-togglelight-desc = Переключить отображение света.
cmd-togglelight-help = Использование: togglelight

cmd-togglefov-desc = Переключить поле зрения для клиента.
cmd-togglefov-help = Использование: togglefov

cmd-togglehardfov-desc = Переключить жесткое поле зрения (для отладки space-station-14#2353).
cmd-togglehardfov-help = Использование: togglehardfov

cmd-toggleshadows-desc = Переключить отображение теней.
cmd-toggleshadows-help = Использование: toggleshadows

cmd-togglelightbuf-desc = Переключить отображение освещения (без FOV).
cmd-togglelightbuf-help = Использование: togglelightbuf

cmd-chunkinfo-desc = Получить информацию о чанке под курсором.
cmd-chunkinfo-help = Использование: chunkinfo

cmd-rldshader-desc = Перезагрузить все шейдеры.
cmd-rldshader-help = Использование: rldshader

cmd-cldbglyr-desc = Переключить слои отладки FOV и света.
cmd-cldbglyr-help= Использование: cldbglyr <слой>: Переключить <слой>
    cldbglyr: Отключить все слои

cmd-key-info-desc = Информация о клавише.
cmd-key-info-help = Использование: keyinfo <Клавиша>

## Команда 'bind'
cmd-bind-desc = Привязать комбинацию клавиш к команде.
cmd-bind-help = Использование: bind { cmd-bind-arg-key } { cmd-bind-arg-mode } { cmd-bind-arg-command }
    Не сохраняет привязки автоматически. Используйте 'svbind' для сохранения.

cmd-bind-arg-key = <ИмяКлавиши>
cmd-bind-arg-mode = <РежимПривязки>
cmd-bind-arg-command = <КомандаВвода>

cmd-net-draw-interp-desc = Переключить отображение сетевой интерполяции.
cmd-net-draw-interp-help = Использование: net_draw_interp

cmd-net-watch-ent-desc = Логировать сетевые обновления для EntityId.
cmd-net-watch-ent-help = Использование: net_watchent <0|EntityUid>

cmd-net-refresh-desc = Запросить полное состояние сервера.
cmd-net-refresh-help = Использование: net_refresh

cmd-net-entity-report-desc = Переключить панель отчета сетевых сущностей.
cmd-net-entity-report-help = Использование: net_entityreport

cmd-fill-desc = Заполнить консоль для отладки.
cmd-fill-help = Заполняет консоль тестовыми сообщениями.

cmd-cls-desc = Очистить консоль.
cmd-cls-help = Очищает консоль от всех сообщений.

cmd-sendgarbage-desc = Отправить мусор на сервер.
cmd-sendgarbage-help = Сервер ответит 'no u'

cmd-loadgrid-desc = Загрузить сетку из файла в существующую карту.
cmd-loadgrid-help = loadgrid <MapID> <Путь> [x y] [rotation] [storeUids]

cmd-loc-desc = Показать абсолютное положение сущности игрока.
cmd-loc-help = loc

cmd-tpgrid-desc = Телепортировать сетку в новое место.
cmd-tpgrid-help = tpgrid <gridId> <X> <Y> [<MapId>]

cmd-rmgrid-desc = Удалить сетку с карты. Нельзя удалить основную сетку.
cmd-rmgrid-help = rmgrid <gridId>

cmd-mapinit-desc = Запустить инициализацию карты.
cmd-mapinit-help = mapinit <mapID>

cmd-lsmap-desc = Показать список карт.
cmd-lsmap-help = lsmap

cmd-lsgrid-desc = Показать список сеток.
cmd-lsgrid-help = lsgrid

cmd-addmap-desc = Добавить новую пустую карту. Если mapID существует, ничего не делает.
cmd-addmap-help = addmap <mapID> [initialize]

cmd-rmmap-desc = Удалить карту из мира. Нельзя удалить nullspace.
cmd-rmmap-help = rmmap <mapId>

cmd-savegrid-desc = Сохранить сетку на диск.
cmd-savegrid-help = savegrid <gridID> <Путь>

cmd-testbed-desc = Загрузить тестовую среду физики на указанной карте.
cmd-testbed-help = testbed <mapid> <тест>

## Команда 'addcomp'
cmd-addcomp-desc = Добавить компонент к сущности.
cmd-addcomp-help = addcomp <uid> <имя компонента>
cmd-addcompc-desc = Добавить компонент к сущности на клиенте.
cmd-addcompc-help = addcompc <uid> <имя компонента>

## Команда 'rmcomp'
cmd-rmcomp-desc = Удалить компонент у сущности.
cmd-rmcomp-help = rmcomp <uid> <имя компонента>
cmd-rmcompc-desc = Удалить компонент у сущности на клиенте.
cmd-rmcompc-help = rmcomp <uid> <имя компонента>

## Команда 'addview'
cmd-addview-desc = Подписаться на просмотр сущности для отладки.
cmd-addview-help = addview <entityUid>
cmd-addviewc-desc = Подписаться на просмотр сущности для отладки (клиент).
cmd-addviewc-help = addview <entityUid>

## Команда 'removeview'
cmd-removeview-desc = Отписаться от просмотра сущности.
cmd-removeview-help = removeview <entityUid>

## Команда 'loglevel'
cmd-loglevel-desc = Изменить уровень логгирования для указанного логгера.
cmd-loglevel-help = Использование: loglevel <логгер> <уровень>
      логгер: Префикс для сообщений.
      уровень: Уровень логгирования (должен соответствовать LogLevel).

cmd-testlog-desc = Записать тестовое сообщение в лог.
cmd-testlog-help = Использование: testlog <логгер> <уровень> <сообщение>
    логгер: Префикс сообщения.
    уровень: Уровень логгирования.
    сообщение: Текст сообщения (в кавычках, если содержит пробелы).

## Команда 'vv'
cmd-vv-desc = Открыть View Variables.
cmd-vv-help = Использование: vv <ID сущности|IoC интерфейс|SIoC интерфейс>

## Команда 'showvelocities'
cmd-showvelocities-desc = Показать угловую и линейную скорости.
cmd-showvelocities-help = Использование: showvelocities

## Команда 'setinputcontext'
cmd-setinputcontext-desc = Установить активный контекст ввода.
cmd-setinputcontext-help = Использование: setinputcontext <контекст>

## Команда 'forall'
cmd-forall-desc = Выполнить команду для всех сущностей с указанным компонентом.
cmd-forall-help = Использование: forall <bql запрос> do <команда...>

## Команда 'delete'
cmd-delete-desc = Удалить сущность с указанным ID.
cmd-delete-help = delete <UID сущности>

# Системные команды
cmd-showtime-desc = Показать серверное время.
cmd-showtime-help = showtime

cmd-restart-desc = Корректно перезапустить сервер (не только раунд).
cmd-restart-help = restart

cmd-shutdown-desc = Корректно выключить сервер.
cmd-shutdown-help = shutdown

cmd-saveconfig-desc = Сохранить конфигурацию сервера.
cmd-saveconfig-help = saveconfig

cmd-netaudit-desc = Показать информацию о безопасности NetMsg.
cmd-netaudit-help = netaudit

# Команды игроков
cmd-tp-desc = Телепортировать игрока в любое место.
cmd-tp-help = tp <x> <y> [<mapID>]

cmd-tpto-desc = Телепортировать игрока/сущность к другому игроку/сущности.
cmd-tpto-help = tpto <имя|uid> [имя|NetEntity]...
cmd-tpto-destination-hint = цель (NetEntity или имя)
cmd-tpto-victim-hint = телепортируемая сущность (NetEntity или имя)
cmd-tpto-parse-error = Не удалось распознать сущность или игрока: {$str}

cmd-listplayers-desc = Показать список подключенных игроков.
cmd-listplayers-help = listplayers

cmd-kick-desc = Кикнуть игрока с сервера.
cmd-kick-help = kick <PlayerIndex> [<Причина>]

# Команда 'spin'
cmd-spin-desc = Заставить сущность вращаться. По умолчанию - родительская сущность игрока.
cmd-spin-help = spin скорость [торможение] [entityUid]

# Команда локализации
cmd-rldloc-desc = Перезагрузить локализацию (клиент и сервер).
cmd-rldloc-help = Использование: rldloc

# Отладочные команды для сущностей
cmd-spawn-desc = Создать сущность указанного типа.
cmd-spawn-help = spawn <прототип> ИЛИ spawn <прототип> <относительный ID> ИЛИ spawn <прототип> <x> <y>
cmd-cspawn-desc = Создать клиентскую сущность у ног.
cmd-cspawn-help = cspawn <тип сущности>

cmd-scale-desc = Изменить размер сущности.
cmd-scale-help = scale <entityUid> <множитель>

cmd-dumpentities-desc = Вывести список сущностей.
cmd-dumpentities-help = Показать список UID и прототипов сущностей.

cmd-getcomponentregistration-desc = Получить информацию о регистрации компонента.
cmd-getcomponentregistration-help = Использование: getcomponentregistration <имя компонента>

cmd-showrays-desc = Переключить отображение лучей физики.
cmd-showrays-help = Использование: showrays <время жизни луча>

cmd-disconnect-desc = Немедленно отключиться от сервера.
cmd-disconnect-help = Использование: disconnect

cmd-entfo-desc = Показать детальную информацию о сущности.
cmd-entfo-help = Использование: entfo <entityuid>
    Можно добавить 'c' перед UID для клиентской сущности.

cmd-fuck-desc = Вызвать исключение
cmd-fuck-help = Вызывает исключение

cmd-showpos-desc = Включить отображение позиций всех сущностей.
cmd-showpos-help = Использование: showpos

cmd-sggcell-desc = Показать сущности в ячейке сетки.
cmd-sggcell-help = Использование: sggcell <gridID> <vector2i>\nvector2i в формате x<int>,y<int>.

cmd-overrideplayername-desc = Изменить имя игрока при подключении.
cmd-overrideplayername-help = Использование: overrideplayername <имя>

cmd-showanchored-desc = Показать закрепленные сущности на тайле.
cmd-showanchored-help = Использование: showanchored

cmd-dmetamem-desc = Вывести члены типа для конфигурации песочницы.
cmd-dmetamem-help = Использование: dmetamem <тип>

cmd-launchauth-desc = Загрузить токены аутентификации для тестирования.
cmd-launchauth-help = Использование: launchauth <имя аккаунта>

cmd-lightbb-desc = Переключить отображение ограничивающих рамок света.
cmd-lightbb-help = Использование: lightbb

cmd-monitorinfo-desc = Информация о мониторах.
cmd-monitorinfo-help = Использование: monitorinfo <id>

cmd-setmonitor-desc = Установить монитор.
cmd-setmonitor-help = Использование: setmonitor <id>

cmd-physics-desc = Показать оверлей физики.
cmd-physics-help = Использование: physics <aabbs / com / contactnormals / contactpoints / distance / joints / shapeinfo / shapes>

cmd-hardquit-desc = Немедленно завершить клиент.
cmd-hardquit-help = Аварийное завершение без уведомления сервера.

cmd-quit-desc = Корректно завершить клиент.
cmd-quit-help = Корректное завершение с уведомлением сервера.

cmd-csi-desc = Открыть C# интерактивную консоль.
cmd-csi-help = Использование: csi

cmd-scsi-desc = Открыть C# интерактивную консоль на сервере.
cmd-scsi-help = Использование: scsi

cmd-watch-desc = Открыть окно просмотра переменных.
cmd-watch-help = Использование: watch

cmd-showspritebb-desc = Переключить отображение границ спрайтов.
cmd-showspritebb-help = Использование: showspritebb

cmd-togglelookup-desc = Переключить отображение границ entitylookup.
cmd-togglelookup-help = Использование: togglelookup

cmd-net_entityreport-desc = Переключить панель сетевых сущностей.
cmd-net_entityreport-help = Использование: net_entityreport

cmd-net_refresh-desc = Запросить полное состояние сервера.
cmd-net_refresh-help = Использование: net_refresh

cmd-net_graph-desc = Переключить панель сетевой статистики.
cmd-net_graph-help = Использование: net_graph

cmd-net_watchent-desc = Логировать сетевые обновления для EntityId.
cmd-net_watchent-help = Использование: net_watchent <0|EntityUid>

cmd-net_draw_interp-desc = Переключить отображение сетевой интерполяции.
cmd-net_draw_interp-help = Использование: net_draw_interp <0|EntityUid>

cmd-vram-desc = Показать статистику использования видеопамяти.
cmd-vram-help = Использование: vram

cmd-showislands-desc = Показать физические острова.
cmd-showislands-help = Использование: showislands

cmd-showgridnodes-desc = Показать узлы разделения сетки.
cmd-showgridnodes-help = Использование: showgridnodes

cmd-profsnap-desc = Создать снимок профилирования.
cmd-profsnap-help = Использование: profsnap

cmd-devwindow-desc = Окно разработчика.
cmd-devwindow-help = Использование: devwindow

cmd-scene-desc = Немедленно изменить сцену/состояние UI.
cmd-scene-help = Использование: scene <имя класса>

cmd-szr_stats-desc = Показать статистику сериализатора.
cmd-szr_stats-help = Использование: szr_stats

cmd-hwid-desc = Показать HWID (идентификатор оборудования).
cmd-hwid-help = Использование: hwid

cmd-vvread-desc = Прочитать значение через View Variables.
cmd-vvread-help = Использование: vvread <путь>

cmd-vvwrite-desc = Изменить значение через View Variables.
cmd-vvwrite-help = Использование: vvwrite <путь>

cmd-vvinvoke-desc = Вызвать метод через View Variables.
cmd-vvinvoke-help = Использование: vvinvoke <путь> [аргументы...]

cmd-dump_dependency_injectors-desc = Показать кэш инъекторов зависимостей IoCManager.
cmd-dump_dependency_injectors-help = Использование: dump_dependency_injectors
cmd-dump_dependency_injectors-total-count = Всего: { $total }

cmd-dump_netserializer_type_map-desc = Показать карту типов NetSerializer.
cmd-dump_netserializer_type_map-help = Использование: dump_netserializer_type_map

cmd-hub_advertise_now-desc = Немедленно отправить объявление на главный хаб.
cmd-hub_advertise_now-help = Использование: hub_advertise_now

cmd-echo-desc = Повторить аргументы в консоль.
cmd-echo-help = Использование: echo "<сообщение>"

## Команда 'vfs_ls'
cmd-vfs_ls-desc = Показать содержимое директории в VFS.
cmd-vfs_ls-help = Использование: vfs_list <путь>
    Пример:
    vfs_list /Assemblies

cmd-vfs_ls-err-args = Требуется ровно 1 аргумент.
cmd-vfs_ls-hint-path = <путь>

cmd-reloadtiletextures-desc = Перезагрузить атлас текстур тайлов.
cmd-reloadtiletextures-help = Использование: reloadtiletextures

cmd-audio_length-desc = Показать длину аудиофайла.
cmd-audio_length-help = Использование: audio_length { cmd-audio_length-arg-file-name }
cmd-audio_length-arg-file-name = <имя файла>

## PVS
cmd-pvs-override-info-desc = Показать информацию о переопределениях PVS для сущности.
cmd-pvs-override-info-empty = Сущность {$nuid} не имеет переопределений PVS.
cmd-pvs-override-info-global = Сущность {$nuid} имеет глобальное переопределение.
cmd-pvs-override-info-clients = Сущность {$nuid} имеет переопределения для сессий {$clients}.
