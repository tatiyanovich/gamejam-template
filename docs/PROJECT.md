# COPYCAT — Project Info (единый файл «всё о проекте»)

> Живой документ. Любое действие по проекту оставляет след в **§9 Work Log**, любое решение — в **§4 Decisions**.
> Правило для Claude: `.claude/rules/docs-maintenance.md`. Правило для людей: то же самое, руками.

## 1. Ключевые факты

| | |
|---|---|
| Игра | **COPYCAT** — comedy stealth‑input game про списывание на кошачьем экзамене |
| Тема джема | Cheat to Win! + обязательная резиновая утка |
| Дедлайн сдачи | **6 сентября 2026, 14:00** (локальное время команды) |
| Старт работы | 5 сентября 2026, ~08:00 (после сна). Ночь 5→6 без сна |
| Платформы | Windows x64, macOS Universal (Apple Silicon + Intel). Standalone, не WebGL |
| Движок | Unity **6000.3.22f1** (установлен; проект был на 6000.3.6f1 — см. D‑12), URP 2D, Entitas + Zenject + UniTask + DOTween + Addressables |
| Разрешение | 1920×1080 референс, 16:9, Fullscreen Window, Canvas Scaler «Scale with screen size» |
| Язык игры | Английский. Дубляж интро — украинский (P1) |
| Репозиторий | этот, ветка `main`. Коммитим часто, маленькими, без веток (нас трое) |
| Документы | `docs/` (этот файл, `GDD.md`, `PLAN.md`, `ART_BIBLE.md`, `BUGS.md`, `prototype/`) |
| Исходники арта | `art/` в корне репо (SVG, скрипты генерации, аудио‑скрипты). В Unity попадают только PNG/WAV |

## 2. Команда и роли

| Кто | Роль | За что отвечает |
|---|---|---|
| **Егор** (Egor Babaev) | Unity‑разработчик, лид | Код, интеграция арта в префабы, билды, финальная сдача. Работает в паре с Claude |
| **Коля** | Гейм‑дизайнер | Контент (вопросы, реплики, тексты UI), баланс по таблице `GDD §11` после каждого плейтеста, Google‑таблица лидерборда + Apps Script деплой, страница на itch.io |
| **Дима** | QA | Билд‑пайплайн мак+вин с утра 5‑го, тест‑план, плейтесты по расписанию, `docs/BUGS.md`, финальный смоук обоих билдов, запись гифок/скриншотов |
| **Claude** | ГД‑консультант / генерация арта, аудио, кода | Генерация всего визуала (SVG→PNG), SFX/VO, вёрстка префабов через Unity MCP, ECS‑код по скиллам `create-*`, поддержка docs |

Принцип: **решения уже приняты (§4). Не обсуждаем — делаем.** Если что‑то не работает — меняем решение, записываем новое в §4 и идём дальше.

## 3. Что уже есть (шаблон)

Проект — форк game‑jam шаблона (`CLAUDE.md`, `README.md`). Используем из него как есть:
- Boot‑флоу и стейт‑машину, загрузочный экран, Fade, окно ошибки, авто‑сейв в JSON, миграции сейва.
- UI‑сервис окон (`WindowBase`, `IUiService`, `WindowDefinition`), кнопки с анимацией, `ProgressBar`.
- Core loop из узлов: `LoopNodeId.StartLaunch` (меню) и `LoopNodeId.Exam` (геймплей, переименован из `Battle` в A1).
- Entitas‑инфраструктуру: фабрики, events, requests, reactive queries, `RefreshSnapshotsFeature`.
- VFX‑пайплайн (`VfxFactory`, `PlayVfxByRequestSystem`), камеру, таймеры, кулдауны (`Common/Cooldown`).

Удалено в A1 (сэмпл про бур): `Gameplay/Drilling`, `Gameplay/Fuel`, `Gameplay/Player`, `Gameplay/Environment`,
`Storage/Systems/RefreshDrillRunSystem.cs`, `UI/Joystick`, поле `BestDrilledDistance` в `GeneralSaveFile`,
`PlayerCharacterKey`, префаб `Player`, окно Joystick из `BootstrapState`, ассеты бесконечного поля и Addressables‑группа `Player`.

## 4. Decisions (журнал решений)

Формат: `D‑NN · дата · решение · почему`. Отменённые решения не удаляем — помечаем `(отменено D‑XX)`.

- **D‑01 · 05.09** · Мяу = способ заставить соседа поднять лапу с ответа (единственный способ увидеть ответ). Мяу **не** снижает подозрение и с растущим шансом привлекает учительницу. · Даёт мяуканью логику и цену; без мяу играть невозможно — требование команды.
- **D‑02 · 05.09** · Наклон (hold Space) — отдельное действие, оно и есть «видимое списывание». Ввод считается только в наклоне. · Игроку нужен мгновенный «спрятаться»; учительница ловит поведение, а не факт открытого листа.
- **D‑03 · 05.09** · Три типа ввода: Strokes (стрелки), Pick (1–4), Word (буквы). Типы фиксированы по вопросам. · Разнообразие без взрыва скоупа; всё — нажатия клавиш, один input‑слой.
- **D‑04 · 05.09** · Утка = бросок‑decoy с диегетическим кулдауном (учительница относит и приносит), 3 использования, потом конфискация. Утка снимает −20 подозрения. · Самая смешная и читаемая версия; кулдаун виден без UI; выполняет требование «утка на столе + на неё можно нажать + это имеет смысл».
- **D‑05 · 05.09** · 20 вопросов, 5 фаз, глобальный звонок 180 с с объявлением на 60 с (числа отменены D‑36, сортировка лидерборда в силе). Лидерборд: ответы ↓, время ↑. · Короткие сессии, эндлесс‑ощущение, редкий «полный проход» как достижение.
- **D‑06 · 05.09** · Вопросы на своём листе — короткий английский текст (флейвор), всё остальное на листах — каракули. · Юмор — канал ценности игры; текст читается в паузах и не конкурирует с копированием, которое идёт по листу соседа.
- **D‑07 · 05.09** · Интро — 5 статичных панелей с наездом камеры, субтитры, озвучка. **Не видео.** · Нет риска кодеков/VideoPlayer на двух платформах, панели правятся по одной, легко пропускать. Визуал интро может отличаться от геймплея.
- **D‑08 · 05.09** · Визуальный стиль — «Chunky Vector Cartoon»: толстые тёмные контуры, плоские заливки, одна тень, огромные глаза с крошечными зрачками; 2D с одноточечной перспективой класса. · Единственный стиль, который Claude стабильно генерирует кодом (SVG) и который одинаково работает для UI и геймплея. Детали — `ART_BIBLE.md`.
- **D‑09 · 05.09** · Генерация арта: SVG, написанный Claude → PNG через `npx @resvg/resvg-js-cli` (фолбэк Chrome headless), пост‑обработка Pillow. Unity AI генерация недоступна (нет моделей/подписки). · Проверено локально 04.09.
- **D‑10 · 05.09** · Анимация — cut‑out (персонажи из слоёв) через DOTween + Animator‑клипы, без покадровой генерации и без Spline. VFX — Unity Particle System, собранные через MCP. · Дёшево, плавно, правится числами.
- **D‑11 · 05.09** · Озвучка интро — macOS `say`: EN‑голос произносит «meow meow», поверх UA `Lesya` через ffmpeg‑фильтр «телефон/гундос». Команда может заменить WAV своей записью без изменений кода. · Есть на машине, ноль зависимостей.
- **D‑12 · 05.09** · Открываем проект в установленном Unity 6000.3.22f1 (апгрейд с 6000.3.6f1, патч‑версия). Если апгрейд ломает импорт — ставим 6000.3.6f1 через Hub и откатываем. · Решить в первый час 5‑го (задача A0).
- **D‑13 · 05.09** · Лидерборд — Google Sheets + Apps Script Web App (POST добавляет строку, GET отдаёт топ‑10 + ранг). Сортировка на стороне скрипта. Таймаут 5 с, офлайн‑фолбэк. · Требование команды; бесплатно; 1 час работы.
- **D‑14 · 05.09** · Микрофон: `UnityEngine.Microphone` + RMS по последним 1024 сэмплам, порог 40 % при масштабе ×420, rising edge, перевзвод 30 %. Legacy Input Manager (в проекте `activeInputHandler: 0`), клавиши по `KeyCode` (физические). · Совместимо с шаблоном; раскладка не важна.
- **D‑15 · 05.09** · Feature freeze **6 сентября 08:00**. После — только баги, баланс и сдача. · Защита от «ещё одну фичу».
- **D‑16 · 05.09** · Шрифты: Luckiest Guy (уже в проекте, заголовки), Patrick Hand (рукописный, листы), Nunito (UI‑текст). Все OFL, качаем с Google Fonts. · Системные шрифты мака нельзя распространять с билдом.
- **D‑17 · 05.09** · Остаёмся на Unity 6000.3.22f1, откат на 6000.3.6f1 не нужен (подтверждение D‑12). · A0: проект открывается без ошибок, компиляция чистая, Jenny‑Gen генерит 223 файла без расхождений с git.
- **D‑18 · 05.09** · Комментариев в коде не пишем вообще (правило в `.claude/rules/code-style.md`). · Имена несут смысл; объяснения живут в `docs/`, а не в исходниках.
- **D‑19 · 05.09** · Фичу `Gameplay/Player` (кинематическое движение, коллизии тела, спавн‑поинт) и `Gameplay/Environment` (бесконечный пол с шейдером) удаляем целиком, а не адаптируем. · Котёнок — статичный cut‑out за партой, он никуда не ходит; `Movement`/`Collisions` остаются как инфраструктура для утки и вьюх.
- **D‑20 · 05.09** · `JoystickInputService` → `KeyboardInputService` (единственная реализация `IInputService`), окно джойстика удалено. · Управление только клавиатура + микрофон (D‑14), тач не поддерживаем.
- **D‑21 · 05.09** · Камеру экзамена создаёт `InitializeExamCameraSystem` через `ICameraFactory.CreateStaticCamera` в начале `GameplayCoreFeature`. · Раньше камера спавнилась вместе с игроком в `InitializePlayerSystem`; камера класса статична, ей не за кем следить.
- **D‑22 · 05.09** · Один сервис конфигов на экзамен: `IExamConfigsService` грузит и `exam_config`, и `difficulty_config`. `IDifficultyService` (A12) берёт фазы у него, своего конфиг‑сервиса не имеет. · Два сервиса на два ассета одной фичи — лишний файл; фазы и вопросы всегда нужны вместе.
- **D‑23 · 05.09** · Длина Strokes/Word и набор типов вопросов — только в `ExamConfig` (контент), в `DifficultyConfig` их нет. Фаза 1 без проверок задана флагом `teacherChecks: false`, а не нулевым `CheckDelay`. · Колонки `GDD §11` «Strokes / Word» и «Типы» — ограничение для автора контента, рантайм их не читает; флаг читается однозначно, ноль — нет.
- **D‑24 · 05.09** · Одна фабрика `IExamFactory` на обе сущности экзамена (`CreateRun`, `CreateQuestion`) вместо двух отдельных фабрик. · Та же логика, что в D‑22: две фабрики на одну фичу — лишний файл, обе читают `IExamConfigsService`.
- **D‑25 · 05.09** · Сущность вопроса получает только payload‑компоненты своего типа (`AnswerStrokes` / `AnswerOptions`+`CorrectOptionIndex` / `AnswerWord`), а завершённость ответа — единая пара `AnswerProgress` >= `AnswerLength`. · Валидация ввода (A5) отбирает вопросы матчером по payload, без `if` по типу внутри цикла; `MarkAnswerCopiedSystem` один на все три типа.
- **D‑26 · 05.09** · Пауза после штампа `COPIED` — `LifetimeLeft` на сущности вопроса (её уничтожает `LifetimeFeature`), число — `questionPauseSeconds` в `ExamConfig` (0.6 с). `SpawnNextQuestionSystem` спавнит следующий вопрос, когда сущности вопроса нет. · Не нужен ни свой таймер, ни своя система очистки; «нет вопроса → спавним» читается однозначно.
- **D‑27 · 05.09** · В скриптах Unity MCP `RunCommand` не пишем `using System.Reflection` — тулза падает с `UNEXPECTED_ERROR`; рефлексия работает через полные имена (`System.Reflection.BindingFlags.…`). · Проверено на A3; правило записано в `.claude/rules/Unity MCP Editor Work.md`.

- **D‑28 · 05.09** · D1 собирается `art/build.mjs` через закреплённый `@resvg/resvg-js` 2.6.2: SVG‑исходники + JSON‑палитра → PNG и автономные SVG с текстом в кривых; локальные шрифты, проверка повторной сборки. · Тот же SVG/resvg‑подход D‑09, воспроизводимый без системных шрифтов; отдельные тестовые объекты @2x, без trim до разделения слоёв. Уточнение D‑16: Luckiest Guy — Apache 2.0; Patrick Hand/Nunito — OFL, лицензии приложены в `art/fonts/`.

- **D‑29 · 05.09** · `IInputService` остаётся механическим (`IsKeyHeld`/`IsKeyPressed`/`GetPointerScreenPosition`), вся раскладка COPYCAT лежит в `InputKeyMap` (`Gameplay/Input/Data`); `M` и `Q` исключены из букв `Word`. · Раскладка — геймплейное знание, ей не место в `Infrastructure`; исключение `M`/`Q` не даёт мяу и утке засчитываться как ошибочный ввод (`GDD §6.3`). Мёртвые `HorizontalAxis`/`VerticalAxis`/`IsRestartInputStarted` из сэмпла бура удалены — `R` на Report Card читается через `IsKeyPressed`.
- **D‑30 · 05.09** · `InputFeature` перенесена из `GlobalLoopInfraTailFeature` в `GlobalLoopInfraHeadFeature`. · `StrokeInput`/`PickInput`/`LetterInput` живут один кадр; в хвосте геймплей читал бы их кадром позже, а в голове ввод виден в том же кадре, в котором нажата клавиша.

- **D‑31 · 05.09** · D1 принят Колей через Егора; замечание к креплению лап соседей из макета закреплено в D5 и E3 (B‑002). · D1 определяет стиль и композицию; полноценные тела/лапы создаются в D5, соединение и перекрытия дополнительно проверяются в движении в E3.
- **D‑32 · 05.09** · Гейт ввода — маркер `AnswerReadable` на сущности вопроса, который каждый кадр ставит `MarkAnswerReadableSystem`; три системы валидации отличаются только матчером payload. · Общее условие «ответ соседа читаем» живёт в одном месте, а не троится по системам (правило `ecs-conventions`: различия — в матчерах, маркер плюс `Mark*`-система); флаг `Watched`, чтобы HUD (B3) и подсветка листа читали его реактивно.
- **D‑33 · 05.09** · В A5 гейт = `LeanHeld` + незавершённый экзамен; условие «лапа соседа поднята» добавляется в `MarkAnswerReadableSystem` вместе с A6. · `PawLifted` и сами соседи создаются в A6 (зависит от A12); поставить условие на несуществующие сущности значило бы заблокировать весь ввод и плейтест до A6. Место врезки — одна строка в `MarkAnswerReadableSystem.Execute`.
- **D‑34 · 05.09** · `WrongInputEvent` несёт только `QuestionIndex` (как `AnswerCopiedEvent`), прогресс ответа при ошибке не сбрасывается. · `GDD §6`: ошибка — штраф подозрения (A9) и треск карандаша (A8), а не потеря набранного; потребители находят текущий вопрос по индексу.

- **D‑35 · 05.09** · D2: пол 1920×670 @1x от y=410 до 1080; точка схода (960,410) на оси доски, как в D1. Девять слоёв @2x без trim, координаты/pivots/порядок и отдельный текст доски в `art/src/d2/layout.json`. · Исходных 420 px пола недостаточно для кадра; отдельные стрелки и текст позволяют анимировать часы и использовать TMP в D11.

- **D‑36 · 05.09** · Экзамен — **12 вопросов** (по фазам 3/2/3/2/2), звонок **120 с** с объявлением на **45 с**, сетка оценок и звёзды пересчитаны под 12 (отменяет числа D‑05, параметры самих фаз не тронуты). · Коля не уверен в 20 вопросах: идеальный проход занимал 161 с из 180, полный экзамен видели бы единицы, а обучающая фаза съедала четверть контента. На 12 вопросах идеал ≈ 91 с из 120 с, попытка укладывается в 1–2 минуты (`GDD §2`, столп 4), `12/12` остаётся мастерским, но достижимым. Выброшены 8 вопросов, ramp длин сохранён: Strokes 2/3/3 → 3 → 4 → 5 → 6, Word `BIRD`/`PURRS`/`CATNIP` (4/5/6), Pick на Q4 и Q8.
- **D‑37 · 05.09** · Скрипт лидерборда живёт в репозитории (`tools/leaderboard/Code.gs`) и правится там, а не в веб‑редакторе; к нему приложены `test.js` (запуск логики под заглушками Apps Script) и `smoke.sh`. Черновик из §6.1 переписан: явный `SPREADSHEET_ID` вместо `getActiveSpreadsheet`, автосоздание листа с шапкой, `LockService` на запись, whitelist оценок `F D C B A A+`, клампы `answers 0–12` / `timeSeconds 0–999`, пустое после фильтра имя → `Anonymous`, любое исключение → JSON с полем `error` вместо HTML. · Черновик падал HTML‑страницей на любой ошибке (клиент C2 не смог бы распарсить ответ), ломался на отсутствующем листе и терял записи при одновременных попытках; без `test.js` каждая правка стоила бы редеплоя. Числа приведены к D‑36 и `GDD §12`.
- **D‑38 · 05.09** · Таблица лидерборда пересоздана под личным Google‑аккаунтом по новой ссылке (`16S1OR1MXPm18jIZbqPbqVxVEikHrkhA_pou0qC3AXH0`), первая таблица (`1a9t4uwVDtyPE-...`) больше не используется. Финальный `Deploy → New deployment → Web app` для Apps Script делается только вручную через веб‑UI, не через `clasp deploy`. · Исходная таблица была создана корпоративным Google‑аккаунтом; анонимный `/exec` систематически возвращал 403 или редирект на логин, даже с манифестом `access: ANYONE_ANONYMOUS`, на двух разных аккаунтах‑деплоерах. Проблема исчезла только после ручного клика Deploy в Apps Script UI на таблице, созданной личным аккаунтом — `clasp deploy` (REST API `projects.deployments.create`) создаёт версию и деплой, но не активирует реальный анонимный HTTP‑доступ веб‑приложения.

- **D‑39 · 05.09** · Доступ к микрофону — инфраструктурный сервис `Infrastructure/Microphone/MicrophoneService` (`Microphone.Start` с loop‑клипом 1 с, RMS по последним 1024 сэмплам, `IsAvailable`), стартует на `IInitializable` бутстрапа и живёт до `Dispose`; масштаб/порог/перевзвод/кулдаун и вся логика мяу — в `Gameplay/Meow` (`MeowConfig`). Состояние «нет микрофона» вьюхи (B2, B3) читают напрямую через `IMicrophoneService.IsAvailable`, не через ECS. · Та же граница, что в D‑29: устройство — инфраструктура (рядом с `KeyboardInputService`), геймплейные числа и правила — фича. Один старт записи на весь запуск снимает вопросы о том, кто включает мик на Attendance и кто в экзамене, и системный запрос доступа на маке случается один раз на загрузке. Оговорка для QA (TP‑01/TP‑02): при явном запрете доступа `Microphone.devices` может остаться непустым, и метр покажет тихий, а не серый мик — игра при этом полностью проходима на `M`.
- **D‑40 · 05.09** · `MeowFeature` добавлена и в `GameplayCoreFeature` (узел `Exam`), и в `LaunchCoreFeature` (узел `StartLaunch`). · Attendance Sheet (B2) тестирует микрофон тем же кодом, что и экзамен: тот же уровень, тот же порог, то же `MeowEvent` — иначе галочка `LOUD ENOUGH!` проверяла бы другую логику, чем игра.
- **D‑41 · 05.09** · Состояние детекции живёт на сущности `MeowSource` (`MicrophoneLevel`, флаг `MeowArmed`) плюс общий кулдаун из `Common/Cooldown` (`OnCooldown`); перевзвод требует `NoneOf(OnCooldown)`, поэтому мяу внутри 0.7 с теряется, а не откладывается (`GDD §8`, TP‑04). Мик и клавиша `M` — две системы с одинаковым эффектом (`EmitMeowOnLoudMicrophoneSystem`, `EmitMeowOnKeyPressedSystem`), `M` работает всегда, а не только без микрофона. · Системы остаются без состояния (правило `ecs-conventions`), а кулдаун переиспользует готовую инфраструктуру вместо своего таймера. `M` всегда живой нужен для плейтестов, greybox‑прогонов A14 и машин без микрофона; риск «жульничества клавишей» перекрыт тем, что мяу и так стоит подозрения.
- **D‑42 · 05.09** · Кейкапы (`Q` в D7, `Space`/`M`/`1–4` в D8) — один шаблон `art/src/d7/keycap.svg` с токеном `{{KEYCAP_LABEL}}`, буква запечена в спрайт шрифтом Luckiest Guy; утка бросается через весь класс налево и падает у доски слева от учительского стола (`art/src/d7/layout.json`). · Подпись кейкапа статична и одна на спрайт — TMP поверх (D‑40) ничего не даёт, а шаблон убирает шесть копий одного рисунка; полёт над учительницей делает бросок читаемым в центре кадра и не прячет утку за правой партой.

- **D‑39 · 05.09** · Штампы (`COPIED`, оценки F–A+ в D8) набираются Luckiest Guy, не Patrick Hand; строка «штампы» в `ART_BIBLE §3` перенесена к Luckiest Guy. · Рукописный шрифт выглядит как пометка ручкой, а не как резиновый штамп; принятый стиль‑лист D1 уже показывал `COPIED!` в Luckiest Guy.
- **D‑40 · 05.09** · Весь текст на листах (шапка, `Student:`, вопрос, `Answer:`, имя соседа, варианты Pick, буквы Word) — TextMeshPro Patrick Hand поверх текстовых‑пустых спрайтов; слоты, кегли и ряды заданы в `art/src/d6/layout.json`. Единственный спрайт с запечённым текстом — `stamp_copied`. · Имя игрока, вопрос и набранный ответ динамические, а буквы Word должны подсвечиваться по одной; та же схема, что доска D2 (`layout.json` → TMP в D11). Добавлен `ring_timer_track` — без трека радиально обрезанное кольцо не читается как таймер.

## 5. Маппинг дизайна на код шаблона

D3: `art/src/d3/` → `art/kitten.mjs` → `art/exports/d3/` (12 спрайтов котёнка); `rig.json` описывает pivots, слои и позы для будущего префаба `Content/Characters/` в D11 и анимации E2.

D4: `art/src/d4/` → `art/teacher.mjs` → `art/exports/d4/` (12 слоёв учительницы); `rig.json` задаёт pivots, сборку и состояния, восемь превью — `art/previews/d4/`. Импорт в `Content/Characters/` — D11, анимация — E1.

D6: `art/src/d6/` → `art/papers.mjs` → `art/exports/d6/` (22 спрайта листов/глифов/штампа/кольца); `layout.json` — слоты TMP, ряды ответа, позиции листов на партах и колец над соседями для префабов `Content/Papers/` (D11), подсветки ввода (A5/B3) и кольца‑таймера (E3).

D7: `art/src/d7/` → `art/duck.mjs` → `art/exports/d7/` (4 кадра утки + `keycap_q`); `layout.json` — позиция утки и кейкапа на парте, три точки дуги броска, место конфискации на учительском столе для префаба `Content/Duck/` (D11), твина броска (E4) и `Gameplay/Duck`.

| Дизайн | Код |
|---|---|
| Меню | `LoopNodeId.StartLaunch`, сцена `Launch`, `LaunchWindow` (Play → Attendance или сразу Exam; Quit → `Application.Quit`) |
| Attendance Sheet | новое окно `AttendanceWindow` (`Assets/Code/UI/Attendance/`), открывается из `LaunchWindow`, если `PlayerName` пуст |
| Intro | новое окно `IntroWindow` (`UI/Intro/`), открывается при входе в Exam, если `IntroSeen == false`; по завершении создаёт `StartExamRequest` |
| Exam | `LoopNodeId.Exam` (переименованный `Battle`), сцена `Gameplay`, фичи в `GameplayCoreFeature` |
| Фичи геймплея | `Gameplay/Exam` (вопросы, прогресс, ответы), `Gameplay/Neighbours` (лапа/окно), `Gameplay/Teacher` (внимание), `Gameplay/Suspicion`, `Gameplay/Meow` (микрофон), `Gameplay/Duck`, `Gameplay/Bell` (таймер), `Gameplay/Difficulty` (фазы) |
| Конфиги | `ExamConfig` (12 вопросов, ключ `exam_config`), `DifficultyConfig` (5 фаз, `difficulty_config`), `MeowConfig` (порог/масштаб), `SuspicionConfig`, `DuckConfig`, `BellConfig` — ScriptableObject в `AddressableResources/Configs/<Feature>/`, группа `Configs` |
| Загрузка конфигов экзамена | `IExamConfigsService` (`Gameplay/Exam/Services`) — грузит `ExamConfig` и `DifficultyConfig`, регистрируется в `BootstrapInstaller.BindConfigServices` |
| Данные вопроса | `QuestionDefinition` (`Gameplay/Exam/Data`), enum‑ы `QuestionType`, `StrokeDirection` (`Gameplay/Exam`), `NeighbourSide` (`Gameplay/Neighbours`) |
| Данные фазы | `DifficultyPhase` (`Gameplay/Difficulty/Data`): `QuestionCount`, `TeacherChecks`, `CheckDelayMinimum/Maximum`, `LookDurationMinimum/Maximum`, `MeowAlertChance`, `PencilSnapAlerts`, `StaringEnabled`, `PawWindow` |
| HUD | `GameplayWindow` переписываем под HUD; подписки через reactive queries (`ISuspicionQuery`, `IExamQuery`, `ITeacherQuery`, `IMeowQuery`, `IDuckQuery`, `IBellQuery`) |
| Report Card | `ResultWindow` переписываем: grade, статы, лидерборд, Retake/Menu |
| Лидерборд | `Infrastructure/Leaderboard/` сервис `ILeaderboardService` (UnityWebRequest), конфиг с URL Apps Script. Серверная часть — `tools/leaderboard/` (`Code.gs`, `test.js`, `smoke.sh`), см. §6.1 |
| Сейв | `GeneralSaveFile`: `PlayerName`, `IntroSeen`, `BestAnswers`, `BestTimeSeconds`. Снапшот обновляет `RefreshExamProgressSystem` в `RefreshSnapshotsFeature` |
| Аудио | `Infrastructure/Audio/` `IAudioService` (SFX по enum `SfxId`, музыка, авто‑дакинг под микрофон) |
| Input | `Gameplay/Input`: флаги `LeanHeld`, `MeowKeyPressed`, `DuckKeyPressed` и однокадровые `StrokeInput` (`StrokeDirection`), `PickInput` (индекс варианта 0–3, клавиши 1–4), `LetterInput` (`char` A–Z без `M`/`Q`) в Input‑контексте; раскладка — `Gameplay/Input/Data/InputKeyMap`, пишет `EmitInputSystem` в `GlobalLoopInfraHeadFeature` |
| Камера экзамена | `InitializeExamCameraSystem` (`Gameplay/Camera/Systems`) — статичная камера из `ICameraFactory.CreateStaticCamera`, префаб `camera_prefab`, регистрируется на узел `Exam` через `BranchCameraRegistrar` |
| Прогресс экзамена | сущность `ExamRun` (`CurrentQuestionIndex`, `AnswersCopied`, `ExamElapsedSeconds`, `ExamFinished`, `ExamOutcomeComponent`) и сущность `Question` — компоненты в `Gameplay/Exam/ExamComponents.cs`, фича `ExamFeature` в `GameplayCoreFeature` |
| Создание сущностей экзамена | `IExamFactory` (`Gameplay/Exam/Services`): `CreateRun` / `CreateQuestion`, биндится в `GameplayInstaller.BindFactories` |
| Исход попытки | enum `ExamOutcome` (`None`/`Passed`/`Caught`/`BellRang`), компонент `ExamOutcomeComponent` на `ExamRun` |
| Микрофон (устройство) | `Infrastructure/Microphone`: `IMicrophoneService` (`IsAvailable`, `GetRootMeanSquare()`), реализация `MicrophoneService` — loop‑клип 1 с, RMS по 1024 сэмплам, биндится в `BootstrapInstaller.BindInputServices` |
| Мяу (детекция) | `Gameplay/Meow`: сущность `MeowSource` (`MicrophoneLevel`, `MeowArmed` + `OnCooldown` из `Common/Cooldown`), системы `InitializeMeowSourceSystem`, `SampleMicrophoneLevelSystem`, `EmitMeowOnLoudMicrophoneSystem`, `EmitMeowOnKeyPressedSystem`, `RearmMeowOnQuietMicrophoneSystem`; `MeowFeature` в `GameplayCoreFeature` и `LaunchCoreFeature` |
| Конфиг мяу | `MeowConfig` (`Gameplay/Meow/Configs`, ключ `meow_config`, группа `Configs`) — `levelScale`, `thresholdLevel`, `rearmLevel`, `cooldownSeconds`; грузит `IMeowConfigsService` |
| Событие «мяу» | `MeowEvent` (флаг, без полей), потребители — A6 (лапа соседа), A8 (шанс разворота), A9 (подозрение), F1 (SFX) |
| Событие «ответ списан» | `AnswerCopiedEvent` (поле `QuestionIndex`), потребитель — `AdvanceExamRunOnAnswerCopiedSystem` |
| Валидация ввода | `MarkAnswerReadableSystem` ставит `AnswerReadable`; `ValidateStrokeInputSystem` / `ValidatePickInputSystem` / `ValidateWordInputSystem` (`Gameplay/Exam/Systems`) — отличаются только матчером payload и компонентом ввода |
| Событие «ошибка ввода» | `WrongInputEvent` (поле `QuestionIndex`), потребители — A8 (треск карандаша) и A9 (`+8` подозрения) |

Сцены: `Boot` (как есть), `Launch` (меню, фон — пустой класс), `Gameplay` (класс с партами; всё, что не UI, — SpriteRenderer‑ы в мире, камера ортографическая, 1920×1080 → размер 19.2×10.8 юнита при PPU 100).

## 6. Внешние сервисы

### 6.1 Google Sheets лидерборд (C1)

| | |
|---|---|
| Таблица | `COPYCAT Leaderboard`, лист `scores`, шапка `name \| answers \| timeSeconds \| grade \| dateUtc` |
| Ссылка | https://docs.google.com/spreadsheets/d/16S1OR1MXPm18jIZbqPbqVxVEikHrkhA_pou0qC3AXH0/edit |
| Скрипт | `tools/leaderboard/Code.gs` — источник правды; в Apps Script копируем оттуда |
| Инструкция деплоя, API, санитизация | `tools/leaderboard/README.md` |
| URL `/exec` | `https://script.google.com/macros/s/AKfycbwRgAcaRgD7gI5p3p-bDdFPsn-8oS_dW3dwgXE7-Gg9KY1W2mzS1H2L8_oy1zEPzJli/exec` — задеплоен и проверен `smoke.sh`, готов для `LeaderboardConfig` (C2) |

Деплой: Extensions → Apps Script → вставить `Code.gs` → запустить `setup()` → Deploy → Web app,
*Execute as* **Me**, *Who has access* **Anyone**. Проверка — `tools/leaderboard/smoke.sh "<url>"`.
Правка после деплоя требует **New version** в Manage deployments, иначе `/exec` отдаёт старый код.
Финальный **Deploy → New deployment → Web app** обязателен вручную через веб‑UI Apps Script — `clasp deploy`
(REST API) создаёт версию и деплой, но не активирует анонимный HTTP‑доступ, даже с манифестом
`access: ANYONE_ANONYMOUS` (проверено на двух аккаунтах и двух таблицах, оба раза `/exec` отдавал 403 или
логин, пока тот же деплой не пересоздали кликом в UI). Таблица пересоздана личным аккаунтом (см. D‑37) —
исходная (`1a9t4uwVDtyPE-...`) принадлежала корпоративному Google‑аккаунту и систематически не давала
анонимный доступ к веб‑приложению.

Ответ на `POST` и `GET` одинаковый: `{ top: [{name, answers, timeSeconds, grade}], rank, total }`,
сортировка `answers` ↓ + `timeSeconds` ↑ (`GDD §12`), `rank == 0` — «не найдено».
`POST` пишет строку и сразу возвращает обновлённый топ, второй запрос не нужен.

Unity: `UnityWebRequest.Post(url, json, "application/json")`, таймаут 5 с, редирект `302`
Apps Script `UnityWebRequest` отрабатывает сам. Логика без деплоя проверяется `node tools/leaderboard/test.js`.

### 6.2 itch.io
Страница создаётся Колей 5‑го вечером (черновик, скрыт). Нужно: название, короткое описание, 5 скриншотов 1920×1080, GIF мяуканья, список управления, «Microphone required (M key fallback)», билды `COPYCAT_win64.zip`, `COPYCAT_mac.zip`.

## 7. Билд и настройки плеера

- Product name `COPYCAT`, company — команда, версия `1.0.<build>`. Иконка — утка с очками (арт D8).
- **macOS:** `Microphone Usage Description` = `COPYCAT needs your microphone to hear you MEOW.` — иначе крэш при `Microphone.Start`. Architecture: Universal. Подписи/нотаризации нет → в описании на itch: «Right‑click → Open» или `xattr -cr COPYCAT.app`.
- **Windows:** x64, Fullscreen Window, `Run in background = true` (микрофон при alt‑tab).
- Сцены в билде: только `Boot` (остальное Addressables). Addressables build перед плеером — пункт чеклиста H1.
- Первый билд обеих платформ — **5 сентября до 12:00** (Дима), чтобы поймать проблемы платформ заранее.

## 8. Пайплайн арта/аудио (кратко, детали — `ART_BIBLE.md`)

Текущий D1: `art/src/d1/*.svg` + `art/palette.json` → `art/build.mjs` (resvg, PNG + SVG с текстом в кривых) → `art/previews/d1/`. Стиль‑лист, две композиции класса и тестовые позы проверены; D1 принят Колей через Егора 05.09. Замечание B‑002 к креплению лап соседей закреплено в D5/E3. Дальнейшая интеграция D11: PNG @2x, trim/pivot и слои → `Assets/AddressableResources/Content/<Feature>/...` → импорт и префабы через Unity MCP `RunCommand` → Addressables группы по фичам (`Copycat_Classroom`, `Copycat_Characters`, `Copycat_UI`, `Copycat_Intro`, `Copycat_Vfx`, `Copycat_Audio`).
Аудио: `art/audio/*.py` (синтез через Python/ffmpeg) и `say` → WAV 44.1 kHz → `Content/Audio/`.

## 9. Work Log (append‑only, одна строка на действие)

Формат: `- YYYY‑MM‑DD HH:MM · Кто · Что сделано · Где/результат`

- 2026‑09‑04 23:30 · Claude · Прочитал HTML‑прототип и GDD v0, инвентаризировал шаблон и инструменты машины (Pillow, ffmpeg, resvg, Chrome, say с голосами EN/UA; Unity AI генерация недоступна) · `docs/prototype/`
- 2026‑09‑04 23:45 · Claude · Написал GDD v1.0, PROJECT.md, PLAN.md, ART_BIBLE.md, BUGS.md, правило `docs-maintenance` · `docs/`, `.claude/rules/`
- 2026‑09‑05 10:25 · Claude · A0: проект открыт в Unity 6000.3.22f1, компиляция без ошибок, Jenny‑Gen отработал (223 файла, git clean) · D‑17, `docs/PLAN.md` A0
- 2026‑09‑05 10:26 · Claude · Добавил правило «не писать комментарии в коде» · `.claude/rules/code-style.md`, D‑18
- 2026‑09‑05 10:45 · Claude · A1: удалил сэмпл бура — код `Gameplay/Drilling`, `Gameplay/Fuel`, `Gameplay/Player`, `Gameplay/Environment`, `UI/Joystick`, `RefreshDrillRunSystem`, `IJoystickInputService`; поле `BestDrilledDistance` из `GeneralSaveFile` · `Assets/Code`
- 2026‑09‑05 10:45 · Claude · A1: удалил ассеты сэмпла (префаб `Player`, спрайты, физматериал, окно джойстика и его `WindowConfig`, конфиги Fuel/Player, шейдер `DesertFloor` с материалом поля, `noise_2`), почистил Addressables — группа `Player` и запись `fuel_config` · `Assets/AddressableResources`, `Assets/AddressableAssetsData`
- 2026‑09‑05 10:45 · Claude · A1: `LoopNodeId.Battle` → `Exam` во всём коде, `BattleSessionChangeDebugAction` → `ExamSessionChangeDebugAction`, `JoystickInputService` → `KeyboardInputService` · D‑20
- 2026‑09‑05 10:45 · Claude · A1: `GameplayCoreFeature` пересобран (scene entities → камера → Movement → UI → teardown), добавлен `InitializeExamCameraSystem`; `GameplayWindow` и `ResultWindow` очищены до заготовок под B3/B5 (из префабов убраны `FuelBar`, `DistanceText`, `BestDistanceText`) · D‑21
- 2026‑09‑05 10:45 · Claude · A1: из сцены `Gameplay` удалены `SpawnPoint` и оба объекта `Ground`; product name = `COPYCAT` · `Assets/Scenes/Gameplay.unity`, `ProjectSettings`
- 2026‑09‑05 10:45 · Claude · A1: Jenny‑Gen после удаления компонентов (200 файлов, минус 23 сгенерированных), компиляция чистая, Play Mode доходит до узла `Exam` без ошибок — камера и HUD‑окно поднимаются · `Assets/Code/Generated`
- 2026‑09‑05 11:05 · Claude · A2: добавил `QuestionType`, `StrokeDirection`, `NeighbourSide`, `QuestionDefinition`, `DifficultyPhase`, `ExamConfig`, `DifficultyConfig` · `Assets/Code/Gameplay/{Exam,Difficulty,Neighbours}`
- 2026‑09‑05 11:05 · Claude · A2: `ExamConfigsService` грузит оба конфига, зарегистрирован в `BootstrapInstaller.BindConfigServices` · D‑22
- 2026‑09‑05 11:05 · Claude · A2: через MCP созданы ассеты `ExamConfig.asset` (20 вопросов из `GDD §13.1`) и `DifficultyConfig.asset` (5 фаз из `GDD §11`), оба в Addressables‑группе `Configs` как `exam_config` / `difficulty_config` · `Assets/AddressableResources/Configs/{Exam,Difficulty}`
- 2026‑09‑05 11:05 · Claude · A2: в `GDD §13.1` заменил неразрывные дефисы на обычные в `nap-to-chaos` и `cardboard-box` — строки в игре именно такие · `docs/GDD.md`
- 2026‑09‑05 11:05 · Claude · A2: проверка — компиляция чистая, Play Mode доходит до `StartLaunch`, `PrepareAssetsState` грузит оба конфига без ошибок · Unity 6000.3.22f1
- 2026‑09‑05 11:40 · Claude · A3: компоненты экзамена (`ExamRun`, `Question`, `AnswerProgress`/`AnswerLength`, `AnswerCopiedEvent`), enum `ExamOutcome` · `Assets/Code/Gameplay/Exam/ExamComponents.cs`, `ExamOutcome.cs`
- 2026‑09‑05 11:40 · Claude · A3: `IExamFactory`/`ExamFactory` (`CreateRun`, `CreateQuestion` с payload по типу вопроса), биндинг в `GameplayInstaller` · D‑24, D‑25
- 2026‑09‑05 11:40 · Claude · A3: системы `InitializeExamRunSystem`, `SpawnNextQuestionSystem`, `AccumulateExamTimeSystem`, `MarkAnswerCopiedSystem`, `AdvanceExamRunOnAnswerCopiedSystem`, `FinishExamOnLastAnswerSystem`; `ExamFeature` добавлена в `GameplayCoreFeature` перед `MovementUpdateFeature` · `Assets/Code/Gameplay/Exam`
- 2026‑09‑05 11:40 · Claude · A3: в `ExamConfig` добавлено `questionPauseSeconds` = 0.6, значение записано в `ExamConfig.asset` через MCP · D‑26
- 2026‑09‑05 11:40 · Claude · A3: Jenny‑Gen (240 файлов), компиляция чистая; Play Mode — Q1 «How many lives does a cat claim to have?» (Strokes, Left, Up|Right), ответ → счётчик 1 и спавн Q2, на 20‑м ответе `ExamFinished` + `Passed`, elapsed замирает, ошибок в консоли нет · Unity 6000.3.22f1
- 2026‑09‑05 11:40 · Claude · Дополнил правило Unity MCP: `using System.Reflection` роняет `RunCommand`, добавлен рецепт чтения ECS‑состояния в Play Mode · `.claude/rules/Unity MCP Editor Work.md`, D‑27
- 2026-09-05 11:12 · Codex · D1: подготовил стиль‑лист, спокойный/опасный кадры класса, тестовые позы котёнка и учительницы, парту/утку, 25 цветов палитры, SVG‑исходники и 10 PNG‑превью; шрифты и лицензии приложены · `art/`, D‑28; художественное утверждение Коли ожидается
- 2026-09-05 11:12 · Codex · D1: визуально проверил кадры в 1920×1080 и 480×270, исправил перекрытие лица/ответа партами; `npm run build` и `npm run check` прошли, PNG/SVG/manifest совпадают · `art/previews/d1/`, `docs/ART_BIBLE.md`, `docs/PLAN.md`; Unity‑сцены и код не менялись
- 2026‑09‑05 11:20 · Claude · A4: Input‑контекст расширен — `LeanHeld`, `MeowKeyPressed`, `DuckKeyPressed`, `StrokeInput`, `PickInput`, `LetterInput`; удалены `HorizontalAxis`/`VerticalAxis` сэмпла · `Assets/Code/Gameplay/Input/InputComponents.cs`
- 2026‑09‑05 11:20 · Claude · A4: раскладка вынесена в `InputKeyMap` + `KeyBinding<T>` (Space, `M`, `Q`, стрелки/WASD, `1–4`, 24 буквы без `M`/`Q`), `EmitInputSystem` пишет флаги и однокадровые компоненты · `Assets/Code/Gameplay/Input/Data`, D‑29
- 2026‑09‑05 11:20 · Claude · A4: `IInputService`/`KeyboardInputService` сведены к `IsKeyHeld`/`IsKeyPressed`/`GetPointerScreenPosition`; `InputFeature` перенесена в `GlobalLoopInfraHeadFeature` · D‑29, D‑30
- 2026‑09‑05 11:20 · Claude · A4: Jenny‑Gen (244 файла), компиляция чистая; в Play Mode `EmitInputSystem` прогнан с подменённым `IInputService` — Space+`M`+`Q`+`↑`+`3`+`T` дают lean/meow/duck/stroke=Up/pick=2/letter=T, одиночный `M` буквы не даёт, `A` даёт stroke=Left и letter=A, отпускание всё снимает; ошибок в консоли нет · Unity 6000.3.22f1
- 2026-09-05 11:40 · Codex · Зафиксировал переданное Егором одобрение Коли: D1 закрыт, замечание к креплению лап соседей занесено в B‑002 и критерии D5/E3; статусы синхронизированы в docs и art/README · D‑31, `docs/PLAN.md`, `docs/ART_BIBLE.md`, `docs/BUGS.md`
- 2026‑09‑05 12:05 · Claude · A5: компоненты `AnswerReadable` и `WrongInputEvent`, системы `MarkAnswerReadableSystem`, `ValidateStrokeInputSystem`, `ValidatePickInputSystem`, `ValidateWordInputSystem`; все четыре добавлены в `ExamFeature` перед `MarkAnswerCopiedSystem` · `Assets/Code/Gameplay/Exam`, D‑32, D‑34
- 2026‑09‑05 12:05 · Claude · A5: гейт ввода пока без лапы соседа — условие `PawLifted` добавляется в A6 одной строкой в `MarkAnswerReadableSystem` · D‑33, `docs/PLAN.md` A6
- 2026‑09‑05 12:05 · Claude · A5: Jenny‑Gen (249 файлов), компиляция чистая; Play Mode с подменённым `IInputService` — Q1–Q8 пройдены: без наклона прогресс не растёт для всех трёх типов, ошибка даёт `WrongInputEvent` без сброса прогресса, верный ввод доводит до COPIED (8/8), ошибок в консоли нет · Unity 6000.3.22f1
- 2026‑09‑05 12:25 · Claude · H1 (частично): выставил `macOS Microphone Usage Description` = «COPYCAT needs your microphone to hear you MEOW.», Architecture = Universal, `Run in Background = true`, Fullscreen Window; прогнал Addressables build (чисто, 24 с) и оба player‑билда — macOS (128 МБ, 0 ошибок) и Windows x64 (105 МБ, 0 ошибок); проверил `Info.plist` — `NSMicrophoneUsageDescription` записан верно; осталось: физический запуск на второй машине (Дима) · `Builds/macOS`, `Builds/Windows`, `docs/PLAN.md` H1
- 2026-09-05 12:10 · Codex · D2: 9 слоёв окружения SVG/PNG @2x, раскладка с pivots и текстом доски, 6 превью; визуальная проверка 1920×1080/480×270, build/check прошли, D1 совпадает побайтно; импорт — D11 · `art/src/d2`, `art/exports/d2`, `art/previews/d2`, D‑35
- 2026-09-05 13:16 · Codex · D3: 12 слоёв котёнка SVG/PNG @2x, rig с pivots/иерархией/позами, нормальные/панические/прищуренные глаза, 7 превью; визуально проверены оба наклона, стыки лап/хвоста и 480×270, build/check прошли, D1/D2 совпадают побайтно · `art/src/d3`, `art/exports/d3`, `art/previews/d3`; D3 закрыт, импорт — D11, runtime — E2

- 2026‑09‑05 13:24 · Claude · Баланс: экзамен переведён с 20 вопросов на 12 — `GDD §11/§12/§13.1`, звонок 120 с и объявление на 45 с, сетка оценок F 0–2 / D 3–5 / C 6–8 / B 9–10 / A 11 / A+ 12, звёзды при 12/12 · D‑36, `docs/GDD.md`
- 2026‑09‑05 13:24 · Claude · Баланс: через MCP обрезан `ExamConfig.asset` до 12 вопросов (сняты 8, у `cardboard-box quantum` сосед L, у `PURRS` — R) и выставлены `questionCount` фаз 3/2/3/2/2 в `DifficultyConfig.asset`; ошибок в консоли нет · `Assets/AddressableResources/Configs/{Exam,Difficulty}`
- 2026‑09‑05 13:24 · Claude · Баланс: обновлены A2/A11/B5/G1 в `PLAN.md`, маппинг конфигов и клэмп `answers` в Apps Script (20 → 12) · `docs/PLAN.md`, `docs/PROJECT.md §5–6`
- 2026-09-05 13:28 · Codex · D4: 12 слоёв учительницы SVG/PNG @2x, rig с pivots и шестью позами, 8 превью; проверены полуоборот, злой взгляд, рука ±8° и зрачки ±6 px, 1920×1080/480×270; build/check прошли, D1–D3 совпадают побайтно · `art/src/d4`, `art/exports/d4`, `art/previews/d4`; D4 закрыт, импорт — D11, анимация — E1
- 2026-09-05 14:05 · Claude · G1: финализировал формулировки 12 вопросов (7 из 12 переписаны), структура тип/сосед/длина payload не менялась; тексты синхронизированы в GDD и конфиге · `docs/GDD.md §13.1`, `Assets/AddressableResources/Configs/Exam/ExamConfig.asset`; G1 закрыт
- 2026‑09‑05 13:40 · Claude · C1: скрипт лидерборда, инструкция деплоя, локальные тесты и smoke‑скрипт · `tools/leaderboard/`, D‑37; деплой и `/exec`‑URL — за владельцем таблицы
- 2026‑09‑05 14:10 · Claude + Егор · C1: таблица лидерборда пересоздана личным аккаунтом (исходная корпоративная блокировала анонимный `/exec`), скрипт задеплоен вручную через Apps Script UI, `smoke.sh` пройден (GET/POST/rank/санитизация), тестовые строки удалены · `tools/leaderboard/`, D‑38, `docs/PROJECT.md §6.1`; C1 закрыт, `/exec`‑URL готов для C2
- 2026‑09‑05 · Claude · H2: написал ручной тест‑план (мик есть/нет/запрещён, тихий/громкий мик, alt‑tab, 16:10/21:9 окна, 10 рестартов подряд, лидерборд офлайн, имя с мусором, Esc в интро, `R` на Report Card) со ссылками на GDD и ожидаемым результатом по каждому кейсу; физический прогон на билдах остаётся за Димой · `docs/TEST_PLAN.md`, `docs/README.md`, `docs/PLAN.md` H2
- 2026-09-05 14:28 · Codex · D5: 8 слоёв соседей SVG/PNG @2x, rig с плечевыми pivots и зеркалированием Fluffy, 6 превью; закрытые/поднятые позы и перекрытия проверены в 1920×1080/480×270, связность тела/лапы — в 111 положениях на персонажа с OutBack overshoot; build/check прошли, D1–D4 совпадают побайтно · `art/src/d5`, `art/exports/d5`, `art/previews/d5`; D5 закрыт, B‑002 исправлен в арте и ожидает E3, импорт — D11
- 2026-09-05 14:45 · Claude · A7: микрофон и мяу — `Infrastructure/Microphone` (`IMicrophoneService`, RMS по 1024 сэмплам, loop‑клип 1 с) и фича `Gameplay/Meow` (`MeowConfig` 420/40/30/0.7, сущность `MeowSource`, 5 систем, событие `MeowEvent`, фолбэк `M`); Jenny‑Gen прогнан, `meow_config` создан и заведён в группу `Configs` · `Assets/Code/Infrastructure/Microphone`, `Assets/Code/Gameplay/Meow`, `Assets/AddressableResources/Configs/Meow/MeowConfig.asset`, D‑39–D‑41; A7 закрыт
- 2026-09-05 14:45 · Claude · A7: проверка в Play Mode на живом микрофоне через MCP — уровень обновляется каждый кадр (`level=3.2…12.5` при ambient), восходящий фронт даёт `MeowEvent` + `OnCooldown`, перевзвод и повторное мяу работают, в консоли чисто; временные значения порога в `MeowConfig` возвращены к 420/40/30/0.7 · Play Mode, `MeowConfig.asset`
- 2026-09-05 14:50 · Claude · D6: 22 спрайта листов и ввода SVG/PNG @2x (свой и соседский лист, 12 глифов стрелок из одного шаблона, круг Pick, штамп `COPIED`, кольцо‑таймер + трек, 4 каракули), `layout.json` со слотами TMP и позициями на партах, 8 превью; проверены три типа ответа, штамп, закрытие ряда ответа лапой D5 в 1920×1080/480×270; `neighbours.mjs` разобран на композитор без изменения вывода, build/check прошли, D1–D5 совпадают побайтно · `art/src/d6`, `art/exports/d6`, `art/previews/d6`, D‑39, D‑40; D6 закрыт, импорт — D11
- 2026-09-05 14:55 · Claude · D7: 5 спрайтов утки SVG/PNG @2x (`duck_idle`, `duck_fly_1/2`, `duck_sad` 140×130, `keycap_q` 64×64 из шаблона `keycap.svg`), `layout.json` с позицией на парте, дугой броска 0.6 с/720° и местом конфискации, 7 превью; проверены парта, hover, 7 положений полёта, учительский стол в 1920×1080/480×270; build/check прошли, D1–D6 совпадают побайтно · `art/src/d7`, `art/exports/d7`, `art/previews/d7`, D‑42; D7 закрыт, импорт — D11, твин — E4
