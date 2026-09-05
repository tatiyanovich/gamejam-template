# COPYCAT — Art Bible & Asset Pipeline

Один стиль на всё: геймплей, UI, интро. Всё генерируется Claude кодом (SVG → PNG) и собирается в Unity через MCP.
Никаких ручных правок в графических редакторах — правим SVG и перегенерируем.

## D1 — готовые материалы для просмотра

[Стиль‑лист 1920×1500](../art/previews/d1/style_sheet.png) · [спокойный кадр](../art/previews/d1/classroom_calm.png) · [опасный кадр](../art/previews/d1/classroom_risk.png) · [миниатюра 480×270](../art/previews/d1/classroom_thumbnail.png) · [исходники и инструкция](../art/README.md).

Подготовлены две тестовые позы котёнка (затылок / наклон с мордой в три четверти), две позы учительницы (спина с мелом / взгляд поверх красных очков), парта и утка. Соседи в кадре — силуэты для проверки композиции; полные слои и состояния остаются задачами D2–D11. Это SVG‑макеты, не скриншоты Unity. D1 принят Колей через Егора 05.09. Замечание B‑002: в макете лапы соседей неестественно крепятся к телам; исправить в D5, проверить в движении в E3.

В `art/palette.json` закреплены все 25 цветов §2. Исходники — `art/src/d1/*.svg`, генератор — `art/build.mjs` с `@resvg/resvg-js` 2.6.2 и lockfile. `npm ci --ignore-scripts`, затем `npm run build` / `npm run check` из `art/`. D1 выдаёт 10 PNG + 10 автономных SVG (текст в кривых) в `art/previews/d1/`; фоновые кадры @1x, отдельные объекты @2x, прозрачные поля поз сохранены. Импорт в Unity, trim, pivot и префабы — D11; общий пайплайн ниже описывает следующие задачи.

Проверено визуально в 1920×1080 и 480×270: позы различимы, лицо учительницы и ответ соседа не перекрыты партой, утка выделяется цветом. Повторная сборка совпадает по PNG/SVG и SHA‑256 manifest. Текст вопроса №1, его два штриха и правила на доске взяты из GDD.

## D2 — окружение класса

[Пустой класс](../art/previews/d2/classroom_empty.png) · [Проверка с позами D1](../art/previews/d2/classroom_risk.png) · [Слои](../art/previews/d2/layer_sheet.png) · [Раскладка](../art/previews/d2/classroom_layout.png).

Готовы 9 отдельных SVG/PNG @2x в `art/exports/d2/`, исходники — `art/src/d2/`. Стена включает окно и циферблат без стрелок; две стрелки отдельно. Доска без текста, строки GDD §13.2 и координаты для TMP — в `layout.json`. Парты без персонажей, листов и утки. Все слои используют PPU 200, Bilinear, no mipmaps, без trim; импорт и Addressables — D11. `art/classroom.mjs` включён в общие `npm run build` / `npm run check`.

Пол уточнён до 1920×670 @1x: начинается в y=410 (горизонт ≈38%) и закрывает кадр до y=1080. Точка схода (960,410) лежит на центральной оси доски, как в принятом D1; геометрический центр доски находится выше горизонта. Размер 1920×420 из исходного списка оставлял бы разрыв. Парты соседей имеют готовый размер 620×260 и поворот ±12° в раскладке; дополнительно умножать их на 0.78 не нужно. Проверочные кадры используют тестовые позы D1 и листы-заглушки; полноценные персонажи и листы — D3–D7, Unity-сцена — D11.

## D3 — котёнок cut-out

[Позы и эмоции](../art/previews/d3/pose_sheet.png) · [Слои и pivots](../art/previews/d3/layer_sheet.png) · [Проверка стыков](../art/previews/d3/motion_sheet.png) · [Кадр](../art/previews/d3/classroom_panic_left.png) · [480×270](../art/previews/d3/classroom_thumbnail.png).

Готовы все 12 спрайтов §5.2 для котёнка: 9 крупных слоёв на общем холсте 470×450 @1x, белок/прищур 110×130, зрачок 24×28. SVG — `art/src/d3/`, PNG @2x и автономные SVG — `art/exports/d3/`. `kitten.mjs` включён в общие build/check. `rig.json` хранит pivots, порядок, позиции, затылок/левый/правый профиль и параметры нормальных/панических/прищуренных глаз. Паника масштабирует белки ×1.2, зрачки ×0.6; зрачки смотрят к соседу. Прищур скрывает зрачки. Наклоны ±12° и ±60 px, дыхание, лапа и хвост сохраняют параметры §7.

Уши отдельные и перекрываются головой, лапы имеют pivot у плеча и незамкнутый контур стыка; проверочные кадры включают крайние/промежуточные положения лапы и хвоста. Без trim, PPU 200, Bilinear, no mipmaps. Формулы сборки и преобразования координат — `art/README.md`. Семь превью проверяют слои, эмоции, стыки и композицию; это SVG-макеты с окружением D2, учительницей D1 и листом-заглушкой. Unity-префаб/Addressables — D11, runtime-анимация — E2.

## D4 — учительница cut-out

[Позы](../art/previews/d4/pose_sheet.png) · [Слои и pivots](../art/previews/d4/layer_sheet.png) · [Рука и взгляд](../art/previews/d4/motion_sheet.png) · [Злой взгляд в классе](../art/previews/d4/classroom_staring.png) · [480×270](../art/previews/d4/classroom_thumbnail.png).

Готовы 12 спрайтов учительницы из §5.2: три тела (спина/полуоборот/фронт), четыре головы (спина/полуоборот/фронт/злая), белок, зрачок, красные очки, указка и рука с мелом. Десять крупных слоёв сохраняют холст 380×600 @1x, белок — 110×130, зрачок — 24×28. Исходники и `rig.json` — `art/src/d4/`, автономные SVG и прозрачные PNG @2x — `art/exports/d4/`. Без trim, PPU 200, Bilinear, no mipmaps. `teacher.mjs` включён в общие build/check.

Полуоборот имеет смещённую вправо морду и асимметричное тело; глаза, зрачки и очки отделены от голов. Staring использует злые брови/рот и сжатые по Y белки/зрачки (×0.36), наклон 6° и scale 1.05. Рука вращается вокруг плеча ±8°, зрачки — X ±6 px; параметры времени сохранены из §7. Восемь превью показывают шесть состояний, слои, стык плеча, крайние/промежуточные движения и четыре кадра класса. Проверена читаемость 1920×1080 и 480×270. Это SVG-макеты с классом D2 и тестовым котёнком D1. Уши входят в головы по списку 12 ассетов; независимое движение ушей в E1 потребует выделения из исходников. Импорт/префаб — D11, runtime-анимация — E1.

## D5 — соседи cut-out

[Позы](../art/previews/d5/pose_sheet.png) · [Слои](../art/previews/d5/layer_sheet.png) · [Стыки и парты](../art/previews/d5/motion_sheet.png) · [Закрыто](../art/previews/d5/classroom_covered.png) · [Открыто](../art/previews/d5/classroom_lifted.png) · [480×270](../art/previews/d5/classroom_thumbnail.png).

Готовы 8 спрайтов §5.2: тело, голова и отдельная лапа каждого соседа, общие белок и зрачок. Whiskerstein — серый табби в круглых очках с бабочкой; Fluffy — более широкий белый пушистый кот с сонными глазами. Очки входят в голову, глаза и зрачки отдельные; сонный взгляд — scale белков (1.05, 0.38). Шесть крупных слоёв на холсте 540×760 @1x, белок 110×130, зрачок 24×28. `art/src/d5/` содержит SVG и `rig.json`, `art/exports/d5/` — прозрачные PNG @2x и автономные SVG. PPU 200, Bilinear, no mipmaps, без trim.

Лапа вращается вокруг плеча (145,335), открытый стык заливкой перекрывает тело. Тело за партой, лапа поверх парты и листа, голова выше лапы. Fluffy зеркалит весь rig. В SVG поднятая лапа имеет угол −70° и Y −40 px; Y +40 из §7 относится к Unity с Y вверх, угол Unity +70° до зеркалирования корня. Длительности и easing §7 сохранены. Шесть превью проверяют позы, слои, переход, выброс OutBack 10% и кадр класса. `check-neighbours.mjs` проверяет связность силуэта тела/лапы в 111 положениях на каждого соседа (0–110%). B‑002 исправлен в арте D5, остаётся проверить runtime в E3. Листы и стрелки — заглушки D6, учительница/котёнок — позы D1, окружение D2; импорт/префабы — D11. Формулы сборки — `art/README.md`.

## D6 — листы и ввод

[Состояния](../art/previews/d6/state_sheet.png) · [Спрайты и pivots](../art/previews/d6/layer_sheet.png) · [Strokes](../art/previews/d6/classroom_strokes.png) · [Pick](../art/previews/d6/classroom_pick.png) · [Word](../art/previews/d6/classroom_word.png) · [COPIED](../art/previews/d6/classroom_copied.png) · [Закрыто лапой](../art/previews/d6/classroom_covered.png) · [480×270](../art/previews/d6/classroom_thumbnail.png).

Готовы 22 спрайта §5.3: `paper_player` 640×460 (перспективный лист с шапкой‑линейками, красным полем, загнутым углом и лапкой‑печатью академии), `paper_neighbour` 480×360, 12 глифов стрелок 84×84 (4 направления × normal/done/wrong: бумага+чернила / OK+белая стрелка / DANGER+белая стрелка), `glyph_pick_circle` 220×100 (карандашная обводка в полтора оборота), `stamp_copied` 300×120 (зелёный штамп `COPIED` + галочка), `ring_timer` и `ring_timer_track` 160×160, `scribble_1..4` 240×40. Исходники — `art/src/d6/`, PNG @2x и автономные SVG — `art/exports/d6/`, `papers.mjs` в общей сборке; глифы собираются из одного шаблона `glyph_arrow.svg` подстановкой угла и цветов состояния. PPU 200, Bilinear, no mipmaps, без trim.

Весь текст листов (шапка `CAT ACADEMY — FINAL EXAM`, `Student: <имя>`, вопрос, `Answer: ____`, имя соседа, варианты Pick, буквы Word) — TextMeshPro Patrick Hand в Unity, в спрайтах текста нет; координаты, размеры шрифта, ряд ответа, ячейки Pick, позиции листов на партах и кольца над головами — `art/src/d6/layout.json`. Штамп — единственный спрайт с запечённым текстом (Luckiest Guy, D‑39). Строки превью взяты из `GDD §13.1` (`exam_samples.json` — только для превью, геймплей читает `ExamConfig`). Лист соседа стоит так, что ряд ответа попадает под лапу D5: `classroom_covered.png` показывает Q9 полностью скрытым. Правильный ввод: глиф `done` на листе соседа и копия на своём листе; ошибка — глиф `wrong` / красная буква; Pick с фазы 3 — круг с alpha 0.35; кольцо — `Image` Filled Radial360 от верха по часовой поверх `ring_timer_track`. Импорт и префабы — D11, привязка к ECS — A5/B3/E3.

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
| Заголовки, логотип, HUD‑цифры, штампы (`COPIED`, оценки) | **Luckiest Guy** | уже в проекте (`Content/UI/Fonts`); Apache 2.0, лицензия в `art/fonts/` |
| Листы, реплики учительницы | **Patrick Hand** | Google Fonts, OFL — скачан в `art/fonts/`; импорт в `Content/UI/Fonts` и TMP‑ассет — при интеграции |
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
| `classroom_floor.png` | 1920×670 | доски в перспективе, y=410…1080; уточнено в D2 |
| `blackboard.png` | 900×300 | рама + доска, без текста (текст — TMP поверх) |
| `teacher_desk.png` | 520×160 | |
| `desk_player.png` | 1920×360 | трапеция, передний план |
| `desk_neighbour_left.png` / `_right.png` | 620×260 | под углом |
| `clock_hand_minute.png` / `clock_hand_hour.png` | 2 файла 12×90 | pivot (6,80) @1x; вращаем кодом |

### 5.2 Персонажи (`Content/Characters/`) — группа `Copycat_Characters`
Котёнок (вид сзади‑сверху, мы видим затылок, уши и лапы на парте + при наклоне — профиль):
`kitten_body_back.png`, `kitten_head_back.png`, `kitten_ear_l/r.png`, `kitten_paw_l/r.png`, `kitten_tail.png`,
`kitten_head_profile_left.png` / `_right.png` (для наклона), `kitten_eye_white.png`, `kitten_pupil.png`, `kitten_eye_squint.png`.
Учительница: `teacher_body_back.png`, `teacher_body_turn.png`, `teacher_body_front.png`, `teacher_head_back.png`, `teacher_head_turn.png`,
`teacher_head_front.png`, `teacher_head_angry.png`, `teacher_eye_white.png`, `teacher_pupil.png`, `teacher_glasses.png`, `teacher_pointer.png`, `teacher_arm_chalk.png`.
Соседи: `nerd_body.png`, `nerd_head.png`, `nerd_paw_cover.png`, `fluffy_body.png`, `fluffy_head.png`, `fluffy_paw_cover.png`, общие `eye_white.png`, `pupil.png`.

Критерий приёмки D5/E3 (B‑002, замечание Коли к D1): лапа естественно выходит из плеча и сохраняет непрерывный силуэт с телом в закрытой и поднятой позах; pivot у плеча, стык слоёв скрыт перекрытием, парта не перерезает лапу. Проверить обе позы и весь переход между ними.

### 5.3 Листы и ввод (`Content/Papers/`) — группа `Copycat_Papers`
`paper_player.png` 640×460, `paper_neighbour.png` 480×360, `glyph_arrow_<up|down|left|right>_<normal|done|wrong>.png` 84×84 (12 файлов),
`glyph_pick_circle.png` 220×100, `stamp_copied.png` 300×120, `ring_timer.png` + `ring_timer_track.png` 160×160 (radial fill в UI поверх трека), `scribble_1..4.png` 240×40.
Все 22 готовы в D6 (`art/exports/d6/`), текст листов — TMP по `art/src/d6/layout.json`.

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
