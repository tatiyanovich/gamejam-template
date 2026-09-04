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
- Core loop из узлов: `LoopNodeId.StartLaunch` (меню) и `LoopNodeId.Battle` (геймплей, **переименуем в `Exam`**).
- Entitas‑инфраструктуру: фабрики, events, requests, reactive queries, `RefreshSnapshotsFeature`.
- VFX‑пайплайн (`VfxFactory`, `PlayVfxByRequestSystem`), камеру, таймеры, кулдауны (`Common/Cooldown`).

Удаляем (сэмпл про бур): `Gameplay/Drilling`, `Gameplay/Fuel`, `Storage/Systems/RefreshDrillRunSystem.cs`, `UI/Joystick`,
поле `BestDrilledDistance` в `GeneralSaveFile`, `PlayerCharacterKey`, префаб `Player`, окно Joystick из `BootstrapState`.

## 4. Decisions (журнал решений)

Формат: `D‑NN · дата · решение · почему`. Отменённые решения не удаляем — помечаем `(отменено D‑XX)`.

- **D‑01 · 05.09** · Мяу = способ заставить соседа поднять лапу с ответа (единственный способ увидеть ответ). Мяу **не** снижает подозрение и с растущим шансом привлекает учительницу. · Даёт мяуканью логику и цену; без мяу играть невозможно — требование команды.
- **D‑02 · 05.09** · Наклон (hold Space) — отдельное действие, оно и есть «видимое списывание». Ввод считается только в наклоне. · Игроку нужен мгновенный «спрятаться»; учительница ловит поведение, а не факт открытого листа.
- **D‑03 · 05.09** · Три типа ввода: Strokes (стрелки), Pick (1–4), Word (буквы). Типы фиксированы по вопросам. · Разнообразие без взрыва скоупа; всё — нажатия клавиш, один input‑слой.
- **D‑04 · 05.09** · Утка = бросок‑decoy с диегетическим кулдауном (учительница относит и приносит), 3 использования, потом конфискация. Утка снимает −20 подозрения. · Самая смешная и читаемая версия; кулдаун виден без UI; выполняет требование «утка на столе + на неё можно нажать + это имеет смысл».
- **D‑05 · 05.09** · 20 вопросов, 5 фаз, глобальный звонок 180 с с объявлением на 60 с. Лидерборд: ответы ↓, время ↑. · Короткие сессии, эндлесс‑ощущение, редкий «полный проход» как достижение.
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

## 5. Маппинг дизайна на код шаблона

| Дизайн | Код |
|---|---|
| Меню | `LoopNodeId.StartLaunch`, сцена `Launch`, `LaunchWindow` (Play → Attendance или сразу Exam; Quit → `Application.Quit`) |
| Attendance Sheet | новое окно `AttendanceWindow` (`Assets/Code/UI/Attendance/`), открывается из `LaunchWindow`, если `PlayerName` пуст |
| Intro | новое окно `IntroWindow` (`UI/Intro/`), открывается при входе в Exam, если `IntroSeen == false`; по завершении создаёт `StartExamRequest` |
| Exam | `LoopNodeId.Exam` (переименованный `Battle`), сцена `Gameplay`, фичи в `GameplayCoreFeature` |
| Фичи геймплея | `Gameplay/Exam` (вопросы, прогресс, ответы), `Gameplay/Neighbours` (лапа/окно), `Gameplay/Teacher` (внимание), `Gameplay/Suspicion`, `Gameplay/Meow` (микрофон), `Gameplay/Duck`, `Gameplay/Bell` (таймер), `Gameplay/Difficulty` (фазы) |
| Конфиги | `ExamConfig` (20 вопросов), `DifficultyConfig` (5 фаз), `MeowConfig` (порог/масштаб), `SuspicionConfig`, `DuckConfig`, `BellConfig` — ScriptableObject в `AddressableResources/Configs/<Feature>/` |
| HUD | `GameplayWindow` переписываем под HUD; подписки через reactive queries (`ISuspicionQuery`, `IExamQuery`, `ITeacherQuery`, `IMeowQuery`, `IDuckQuery`, `IBellQuery`) |
| Report Card | `ResultWindow` переписываем: grade, статы, лидерборд, Retake/Menu |
| Лидерборд | `Infrastructure/Leaderboard/` сервис `ILeaderboardService` (UnityWebRequest), конфиг с URL Apps Script |
| Сейв | `GeneralSaveFile`: `PlayerName`, `IntroSeen`, `BestAnswers`, `BestTimeSeconds`. Снапшот обновляет `RefreshExamProgressSystem` в `RefreshSnapshotsFeature` |
| Аудио | `Infrastructure/Audio/` `IAudioService` (SFX по enum `SfxId`, музыка, авто‑дакинг под микрофон) |
| Input | `Gameplay/Input` расширяем: `LeanHeld`, `StrokeInput`, `PickInput`, `LetterInput`, `MeowKeyPressed`, `DuckKeyPressed` в Input‑контексте |

Сцены: `Boot` (как есть), `Launch` (меню, фон — пустой класс), `Gameplay` (класс с партами; всё, что не UI, — SpriteRenderer‑ы в мире, камера ортографическая, 1920×1080 → размер 19.2×10.8 юнита при PPU 100).

## 6. Внешние сервисы

### 6.1 Google Sheets лидерборд (Коля деплоит, Claude пишет скрипт)
1. Создать таблицу `COPYCAT Leaderboard`, лист `scores`, шапка: `name | answers | timeSeconds | grade | dateUtc`.
2. Extensions → Apps Script, вставить код ниже, Deploy → **Web app**, Execute as *Me*, Access **Anyone**. Скопировать URL `/exec` в `LeaderboardConfig`.
3. Проверить `curl -L "<url>?top=10&name=test"`.

```javascript
const SHEET = 'scores';
function doPost(e) {
  const body = JSON.parse(e.postData.contents);
  const name = String(body.name || 'Anonymous').slice(0, 12).replace(/[^\w \-]/g, '');
  const answers = Math.max(0, Math.min(20, Number(body.answers) | 0));
  const time = Math.max(0, Math.min(999, Number(body.timeSeconds) || 0));
  const grade = String(body.grade || 'F').slice(0, 2);
  SpreadsheetApp.openById(SpreadsheetApp.getActiveSpreadsheet().getId())
    .getSheetByName(SHEET).appendRow([name, answers, time, grade, new Date().toISOString()]);
  return doGet({ parameter: { top: '10', name: name, answers: String(answers), timeSeconds: String(time) } });
}
function doGet(e) {
  const p = (e && e.parameter) || {};
  const top = Math.min(50, Number(p.top) || 10);
  const rows = SpreadsheetApp.getActiveSpreadsheet().getSheetByName(SHEET).getDataRange().getValues().slice(1)
    .map(r => ({ name: r[0], answers: Number(r[1]), timeSeconds: Number(r[2]), grade: r[3] }))
    .sort((a, b) => b.answers - a.answers || a.timeSeconds - b.timeSeconds);
  let rank = 0;
  if (p.name !== undefined) {
    rank = 1 + rows.findIndex(r => r.name === p.name && r.answers === Number(p.answers) && r.timeSeconds === Number(p.timeSeconds));
  }
  const out = { top: rows.slice(0, top), rank: rank, total: rows.length };
  return ContentService.createTextOutput(JSON.stringify(out)).setMimeType(ContentService.MimeType.JSON);
}
```
Unity: `UnityWebRequest.Post(url, json, "application/json")`, редиректы Apps Script (302) UnityWebRequest следует сам. Таймаут 5 с.

### 6.2 itch.io
Страница создаётся Колей 5‑го вечером (черновик, скрыт). Нужно: название, короткое описание, 5 скриншотов 1920×1080, GIF мяуканья, список управления, «Microphone required (M key fallback)», билды `COPYCAT_win64.zip`, `COPYCAT_mac.zip`.

## 7. Билд и настройки плеера

- Product name `COPYCAT`, company — команда, версия `1.0.<build>`. Иконка — утка с очками (арт D8).
- **macOS:** `Microphone Usage Description` = `COPYCAT needs your microphone to hear you MEOW.` — иначе крэш при `Microphone.Start`. Architecture: Universal. Подписи/нотаризации нет → в описании на itch: «Right‑click → Open» или `xattr -cr COPYCAT.app`.
- **Windows:** x64, Fullscreen Window, `Run in background = true` (микрофон при alt‑tab).
- Сцены в билде: только `Boot` (остальное Addressables). Addressables build перед плеером — пункт чеклиста H1.
- Первый билд обеих платформ — **5 сентября до 12:00** (Дима), чтобы поймать проблемы платформ заранее.

## 8. Пайплайн арта/аудио (кратко, детали — `ART_BIBLE.md`)

`art/src/*.svg` → `art/build.sh` (resvg → PNG @2x, Pillow trim/sheet) → `Assets/AddressableResources/Content/<Feature>/...` → импорт и префабы через Unity MCP `RunCommand` → Addressables группы по фичам (`Copycat_Classroom`, `Copycat_Characters`, `Copycat_UI`, `Copycat_Intro`, `Copycat_Vfx`, `Copycat_Audio`).
Аудио: `art/audio/*.py` (синтез через Python/ffmpeg) и `say` → WAV 44.1 kHz → `Content/Audio/`.

## 9. Work Log (append‑only, одна строка на действие)

Формат: `- YYYY‑MM‑DD HH:MM · Кто · Что сделано · Где/результат`

- 2026‑09‑04 23:30 · Claude · Прочитал HTML‑прототип и GDD v0, инвентаризировал шаблон и инструменты машины (Pillow, ffmpeg, resvg, Chrome, say с голосами EN/UA; Unity AI генерация недоступна) · `docs/prototype/`
- 2026‑09‑04 23:45 · Claude · Написал GDD v1.0, PROJECT.md, PLAN.md, ART_BIBLE.md, BUGS.md, правило `docs-maintenance` · `docs/`, `.claude/rules/`
