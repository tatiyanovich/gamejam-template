# COPYCAT — Art Bible & Asset Pipeline

Один стиль на всё: геймплей, UI, интро. Всё генерируется Claude кодом (SVG → PNG) и собирается в Unity через MCP.
Никаких ручных правок в графических редакторах — правим SVG и перегенерируем.

## D1 — готовые материалы для просмотра

[Стиль‑лист 1920×1500](../art/previews/d1/style_sheet.png) · [спокойный кадр](../art/previews/d1/classroom_calm.png) · [опасный кадр](../art/previews/d1/classroom_risk.png) · [миниатюра 480×270](../art/previews/d1/classroom_thumbnail.png) · [исходники и инструкция](../art/README.md).

Подготовлены две тестовые позы котёнка (затылок / наклон с мордой в три четверти), две позы учительницы (спина с мелом / взгляд поверх красных очков), парта и утка. Соседи в кадре — силуэты для проверки композиции; полные слои и состояния остаются задачами D2–D11. Это SVG‑макеты, не скриншоты Unity. Материалы готовы; утверждение Коли пока не зафиксировано.

В `art/palette.json` закреплены все 25 цветов §2. Исходники — `art/src/d1/*.svg`, генератор — `art/build.mjs` с `@resvg/resvg-js` 2.6.2 и lockfile. `npm ci --ignore-scripts`, затем `npm run build` / `npm run check` из `art/`. D1 выдаёт 10 PNG + 10 автономных SVG (текст в кривых) в `art/previews/d1/`; фоновые кадры @1x, отдельные объекты @2x, прозрачные поля поз сохранены. Импорт в Unity, trim, pivot и префабы — D11; общий пайплайн ниже описывает следующие задачи.

Проверено визуально в 1920×1080 и 480×270: позы различимы, лицо учительницы и ответ соседа не перекрыты партой, утка выделяется цветом. Повторная сборка совпадает по PNG/SVG и SHA‑256 manifest. Текст вопроса №1, его два штриха и правила на доске взяты из GDD.

## 1. Стиль: «Chunky Vector Cartoon»

Референс‑ощущение: флэт‑мультяшность уровня Adventure Time / Cartoon Network shorts, кооп‑симуляторные коты с огромными глазами.
Почему этот стиль: он **полностью** выражается примитивами SVG (path, круги, толстые stroke, одна тень) и выглядит законченно без текстур, светотени и деталей — то, что генерация кодом делает стабильно.

Правила (жёсткие):
1. **Контур** — один цвет `INK #2A1F1A`, толщина 10 px на масштабе 1920×1080 для персонажей, 6 px для мелких элементов, 14 px для UI‑панелей. `stroke-linejoin: round`, `stroke-linecap: round`.
2. **Заливки плоские.** Одна тень на объект: та же форма, сдвинутая вниз‑вправо на 4–6 %, цветом на 18 % темнее, обрезанная по силуэту (`clipPath`). Никаких градиентов на персонажах. Мягкий радиальный градиент разрешён только на фоне (свет из окна).
3. **Формы «сквошены»**: круги слегка эллиптичны, прямоугольники со скруглением ≥ 12 px, углы «пожёваны» лёгкой асимметрией (±2 %). Идеальная геометрия запрещена — она выглядит как клипарт.
4. **Глаза**: белые эллипсы размером 30–40 % ширины головы, зрачки — точки 12 % от глаза, всегда смотрят в одну сторону. Паника = глаза ×1.2, зрачки ×0.6. Прищур = глаз‑полоска.
5. **Морды**: нос — маленький треугольник, рот — «w» линией, усы — по 3 линии, уши — треугольники с внутренним розовым.
6. **Перспектива**: одноточечная, линия горизонта на 38 % высоты экрана, точка схода — центр доски. Пол — доски, сходящиеся к центру. Своя парта — трапеция во всю ширину низа (высота 32 % экрана). Парты соседей — под углом 12° влево/вправо, уменьшены до 78 %. Учительница — 55 % высоты котёнка (она дальше).
7. **Текст на листах** — шрифт Patrick Hand, чернила `#3B3A8C` (синяя ручка), каракули — линии `#8C8C8C` толщиной 4 px с волной.
8. **Одноцветные тени под объектами** на парте: эллипс `INK` с opacity 0.12.

## 2. Палитра

| Имя | HEX | Где |
|---|---|---|
| INK | `#2A1F1A` | все контуры, текст UI тёмный |
| PAPER | `#FFF6E3` | листы, панели UI |
| PAPER_SHADE | `#EADFC6` | тень на бумаге |
| WALL | `#BFD8D2` | стена класса |
| WALL_LIGHT | `#DCEBE7` | свет из окна |
| BOARD | `#2E5B4F` | доска |
| BOARD_FRAME | `#8B5A34` | рама доски, дерево тёмное |
| WOOD | `#C48A5A` | парты |
| WOOD_SHADE | `#9E6A3F` | тень парт |
| FLOOR | `#D9B48A` | пол |
| KITTEN | `#F3A24A` | котёнок (рыжий) |
| KITTEN_LIGHT | `#FFE7C2` | морда/грудка |
| TEACHER | `#8CA0C8` | учительница (серо‑голубая) |
| TEACHER_DRESS | `#5B4B8A` | платье |
| GLASSES_RED | `#E0453B` | очки учительницы |
| NERD | `#9E9E9E` | Whiskerstein (серый табби) |
| FLUFFY | `#F5F1EA` | Fluffy (белый) |
| DUCK | `#FFD83D` | утка |
| DUCK_BEAK | `#FF8A3D` | клюв |
| UI_ACCENT | `#FFD23F` | кнопки, выделение |
| DANGER | `#E84C4C` | ошибки, красный виньет |
| WARN | `#FF9A3D` | оранжевый виньет, 50–80 % подозрения |
| OK | `#4FCB7A` | верный ввод, LOUD ENOUGH |
| PENCIL_INK | `#3B3A8C` | рукописный текст |

Шкала подозрения: градиент `#FFD85D → #FF9A55 → #E84C4C`.

## 3. Типографика

| Роль | Шрифт | Источник |
|---|---|---|
| Заголовки, логотип, HUD‑цифры | **Luckiest Guy** | уже в проекте (`Content/UI/Fonts`); Apache 2.0, лицензия в `art/fonts/` |
| Листы, штампы, реплики учительницы | **Patrick Hand** | Google Fonts, OFL — скачан в `art/fonts/`; импорт в `Content/UI/Fonts` и TMP‑ассет — при интеграции |
| Текст UI, статы, лидерборд | **Nunito Bold** | Google Fonts, OFL — исходный variable font и статический `wght=700` в `art/fonts/` |

Стрелки на листах рисуем как глифы (SVG), не шрифтом.

## 4. Пайплайн генерации

```
art/
  src/            # SVG‑исходники, по одному объекту/слою на файл (kitten_head.svg, teacher_back.svg …)
  palette.json    # цвета из §2, подставляются в SVG через {{INK}} шаблоны
  build.py        # рендер всех SVG → PNG @2x (resvg), trim, упаковка в Assets
  audio/          # синтез SFX/VO (python + ffmpeg + say)
```

1. Claude пишет SVG с viewBox в «экранных» пикселях 1920×1080 (объект — в своём естественном размере на экране).
2. `npx -y @resvg/resvg-js-cli --scale 2 in.svg out.png` → PNG @2x (для чётких контуров при масштабировании UI). Фолбэк: `Google Chrome --headless --screenshot`.
3. Pillow: trim прозрачных краёв (запоминаем pivot), объединение кадров в sheet при необходимости.
4. Копирование в `Assets/AddressableResources/Content/<Feature>/Sprites/<name>.png`.
5. MCP `RunCommand`: `TextureImporter` → Sprite, PPU **200** (потому что @2x), Bilinear, no mipmaps, pivot по данным trim; сборка префабов; Addressables entry по правилам репо: суффикс типа — `_texture` для текстур, `_prefab` для префабов, `_config` для SO.
6. Проверка: скриншот `Unity_SceneView_Capture2DScene` → Claude смотрит и правит SVG.

Итерация одного ассета ≈ 3–6 минут. Никаких ручных доработок в редакторах.

## 5. Список ассетов

### 5.1 Класс (`Content/Classroom/`) — группа `Copycat_Classroom`
| Файл | Размер (px, @1x) | Заметки |
|---|---|---|
| `classroom_wall.png` | 1920×700 | стена + окно со светом + часы (циферблат без стрелок) |
| `classroom_floor.png` | 1920×420 | доски в перспективе |
| `blackboard.png` | 900×300 | рама + доска, без текста (текст — TMP поверх) |
| `teacher_desk.png` | 520×160 | |
| `desk_player.png` | 1920×360 | трапеция, передний план |
| `desk_neighbour_left.png` / `_right.png` | 620×260 | под углом |
| `clock_hands.png` | 2 файла 12×90 | вращаем кодом |

### 5.2 Персонажи (`Content/Characters/`) — группа `Copycat_Characters`
Котёнок (вид сзади‑сверху, мы видим затылок, уши и лапы на парте + при наклоне — профиль):
`kitten_body_back.png`, `kitten_head_back.png`, `kitten_ear_l/r.png`, `kitten_paw_l/r.png`, `kitten_tail.png`,
`kitten_head_profile_left.png` / `_right.png` (для наклона), `kitten_eye_white.png`, `kitten_pupil.png`, `kitten_eye_squint.png`.
Учительница: `teacher_body_back.png`, `teacher_body_turn.png`, `teacher_body_front.png`, `teacher_head_back.png`, `teacher_head_turn.png`,
`teacher_head_front.png`, `teacher_head_angry.png`, `teacher_eye_white.png`, `teacher_pupil.png`, `teacher_glasses.png`, `teacher_pointer.png`, `teacher_arm_chalk.png`.
Соседи: `nerd_body.png`, `nerd_head.png`, `nerd_paw_cover.png`, `fluffy_body.png`, `fluffy_head.png`, `fluffy_paw_cover.png`, общие `eye_white.png`, `pupil.png`.

### 5.3 Листы и ввод (`Content/Papers/`) — группа `Copycat_Papers`
`paper_player.png` 640×460, `paper_neighbour.png` 480×360, `glyph_arrow_up/down/left/right.png` 84×84 (3 состояния каждый: normal/done/wrong → 12 файлов),
`glyph_pick_circle.png`, `stamp_copied.png` 300×120, `ring_timer.png` 160×160 (radial fill в UI), `scribble_1..4.png`.

### 5.4 Утка (`Content/Duck/`)
`duck_idle.png` 140×130, `duck_fly_1.png`, `duck_fly_2.png`, `duck_sad.png`, `keycap_q.png` 64×64.

### 5.5 UI (`Content/UI/Copycat/`) — группа `Copycat_UI`
`panel_paper_9slice.png` 128×128 (borders 32), `button_yellow_9slice.png` (normal/hover/pressed), `bar_frame.png`, `bar_fill.png`, `meow_circle.png` 240×240,
`meow_threshold_line.png`, `chip_hud.png` 9‑slice, `star_filled.png`/`star_empty.png` 96×96, `stamp_grade_F/D/C/B/A/A+.png` 220×220, `logo_copycat.png` 900×300,
`icon_app.png` 1024×1024 (утка в очках), `vignette_radial.png` 512×512 (белый радиальный, красится кодом), `keycap_space.png`, `keycap_m.png`, `keycap_1..4.png`.

### 5.6 Интро (`Content/Intro/`) — группа `Copycat_Intro`
`intro_panel_1..5.png` 2304×1296 (на 20 % больше кадра для наезда), `letterbox.png`, `film_grain.png` 512×512 (tile, Pillow noise).

### 5.7 VFX‑спрайты (`Content/Vfx/Sprites/`) — группа `Copycat_Vfx`
`sweat_drop.png`, `note.png`, `exclaim.png`, `question.png`, `chalk_dust.png`, `sparkle.png`, `heart.png` — все 64×64.

## 6. Интро — визуальные отличия
Та же палитра и контур, но: фон затемнён на 35 %, добавлен виньет, леттербокс 2.39:1, плёночное зерно (opacity 0.08), лёгкая хроматическая аберрация не делаем (дорого). Панель 3 (крупный план котёнка) — единственная с драматичным светом: радиальный градиент от лица.

## 7. Анимация (cut‑out)

Все персонажи — иерархия GameObject'ов со SpriteRenderer‑ами; анимируем `localPosition/localRotation/localScale` DOTween‑секвенциями, которые запускают Behaviour‑ы по событиям reactive‑query (правило `view-reactive-queries`).

| Персонаж | Состояние | Что двигается |
|---|---|---|
| Учительница | Writing | рука с мелом: rotation ±8°, loop 0.6 с; спина слегка покачивается |
| | Turning (0.3 с) | уши scale 1→1.25→1; swap head_back→head_turn на 0.15 с→head_front; пузырь `Hmm?` |
| | Watching | зрачки: X от −6 до +6 px, случайные паузы 0.3–0.6 с; глаза «останавливаются» на игроке, когда он наклонён |
| | Staring | swap head_angry; тело наклон вперёд 6°, scale 1.05; виньет красный |
| | Distracted | движение по X к месту утки (0.8 с), там 2 с, назад (0.8 с) с уткой в руке (swap sprite), плюх на парту |
| Котёнок | Idle | дыхание scale 1↔1.02, 1.6 с; хвост rotation ±10° |
| | Lean | тело rotation −12°/+12°, сдвиг X ±60 px, swap head_back→head_profile; глаза‑паника при Watching |
| | Typing | лапа rotation 0→−15→0 за 0.12 с на каждый верный ввод |
| Сосед | Paw lift | лапа rotation −70° + Y +40 за 0.25 с (OutBack); голова вздрагивает на мяу |
| | Cover | обратное за 0.35 с, за 1 с до конца — «дрожит» |
| Утка | Idle | bob Y ±6 px, 1.2 с; hover scale 1.1 |
| | Throw | парабола по 3 точкам 0.6 с, rotation 720°, swap fly_1/fly_2 каждые 0.1 с, писк на приземлении |

## 8. VFX (Unity Particle System через MCP)

| Эффект | Триггер | Параметры |
|---|---|---|
| Пот котёнка | Watching, пока наклонён | 3–5 капель/с, gravity 2, lifetime 0.6, из висков |
| `!` над учительницей | Turning/Alerted | burst 1, scale 0→1.2→1, lifetime 0.5 |
| Ноты смеха | Бросок утки | burst 12 нот от парт соседей, вверх, lifetime 1.2 |
| Пыль | Приземление утки | burst 8, серые круги, lifetime 0.4 |
| Мел | Writing | 2/с, мелкие белые точки, lifetime 0.3 |
| Звёздочки | COPIED | burst 10 звёзд от штампа, lifetime 0.6, + punch‑scale счётчика |
| Виньет | Watching/Staring | UI Image `vignette_radial`, цвет WARN/DANGER, alpha пульс 0.35↔0.6, период 0.4 с |
| Шейк камеры | CAUGHT | `CameraShakeTypeId` из шаблона, амплитуда 0.4, 0.5 с |
