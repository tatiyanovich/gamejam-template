# COPYCAT — лидерборд на Google Sheets (задача C1)

Веб‑приложение Apps Script, привязанное к таблице команды. Клиент (`ILeaderboardService`, задача C2)
шлёт `POST` с результатом попытки и получает в ответ топ‑10 и свой ранг одним запросом.

| | |
|---|---|
| Таблица | https://docs.google.com/spreadsheets/d/16S1OR1MXPm18jIZbqPbqVxVEikHrkhA_pou0qC3AXH0/edit |
| Лист | `scores`, шапка `name \| answers \| timeSeconds \| grade \| dateUtc` |
| Скрипт | [`Code.gs`](Code.gs) — единственный источник правды, правим здесь и копируем в Apps Script |
| URL деплоя (`/exec`) | `https://script.google.com/macros/s/AKfycbwRgAcaRgD7gI5p3p-bDdFPsn-8oS_dW3dwgXE7-Gg9KY1W2mzS1H2L8_oy1zEPzJli/exec` — проверено `smoke.sh`, для `LeaderboardConfig` в C2 |

> Таблица пересоздана (D‑37 отменяет первоначальную ссылку на `1a9t4uwVDtyPE-...`): исходная таблица принадлежала корпоративному
> Google‑аккаунту, и Apps Script систематически возвращал 403/редирект на логин анонимным запросам, даже с манифестом
> `access: ANYONE_ANONYMOUS` и после смены деплоящего аккаунта. Новая таблица создана личным аккаунтом — тот же `Code.gs`,
> анонимный доступ подтверждён.

## Деплой (делает владелец таблицы)

1. Открыть таблицу → **Extensions → Apps Script**.
2. Удалить содержимое `Code.gs` в редакторе, вставить целиком содержимое [`Code.gs`](Code.gs) из репозитория. Сохранить.
3. Выбрать в выпадающем списке функцию `setup`, нажать **Run**, выдать разрешения. Появится лист `scores` с шапкой
   (если его ещё нет — `sheet()` в скрипте создаёт его и сам, при первом `doGet`/`doPost`).
4. **Deploy → New deployment** → шестерёнка ⚙️ рядом с "Select type" → **Web app**:
   - *Execute as* — **Me**;
   - *Who has access* — **Anyone** (именно `Anyone`, не `Anyone with Google account` — иначе игра получит HTML логина вместо JSON).
5. Скопировать URL вида `https://script.google.com/macros/s/<id>/exec` — вписать в таблицу выше, в `docs/PROJECT.md §6.1`.
6. Проверить: `./smoke.sh "<url>"` — четыре запроса, ответом всегда JSON. Строки `smoke test`/`Anonymous` потом удалить из листа `scores`.

> **Важно:** этот финальный шаг (Deploy → New deployment → Web app) обязательно делать вручную через веб‑интерфейс
> Apps Script. `clasp deploy` (Apps Script REST API `projects.deployments.create`) технически создаёт деплой и принимает
> манифест с `access: ANYONE_ANONYMOUS`, но реально не активирует анонимный HTTP‑доступ — проверено на двух разных
> Google‑аккаунтах и двух таблицах, оба раза `/exec` отдавал 403/логин, пока тот же деплой не пересоздали кликом в UI.
> Правки скрипта после деплоя требуют **Deploy → Manage deployments → Edit → Version: New version**,
> иначе `/exec` продолжит отдавать старый код по тому же URL.

## API

`POST` (тело `application/json`) и `GET` возвращают одну и ту же структуру:

```json
{ "top": [{ "name": "Egor", "answers": 12, "timeSeconds": 91.37, "grade": "A+" }], "rank": 3, "total": 41 }
```

- `top` — отсортировано по `answers` ↓, затем `timeSeconds` ↑ (`GDD §12`), длина — параметр `top` (по умолчанию 10, максимум 50).
- `rank` — позиция отправленной попытки, начиная с 1; **0 означает «не найдено»** (офлайн‑строка, чужой запрос).
- `error` появляется в ответе, только если скрипт упал; `top` при этом пустой. Клиент показывает `Leaderboard offline`.

`POST` пишет строку и сразу отдаёт обновлённый топ — второй запрос за рангом не нужен.

Санитизация входа (клиенту доверять нельзя, таблица публично‑пишущая):

| Поле | Правило |
|---|---|
| `name` | только `A–Z a–z 0–9 _ - ` и пробел, ≤ 12 символов, пусто → `Anonymous` |
| `answers` | целое, зажато в `0…12` (`D‑36`) |
| `timeSeconds` | `0…999`, округление до 0.01 |
| `grade` | только `F D C B A A+` (`GDD §12`), иначе `F` |

Одновременные записи сериализуются через `LockService` (таймаут 10 с).

## Тесты без деплоя

```
node test.js
```

Гоняет `Code.gs` под заглушками `SpreadsheetApp`/`LockService`/`ContentService`: сортировка, ранг,
клампы, мусорные имена, битый JSON, создание шапки. Прогонять после любой правки скрипта —
дешевле, чем ловить это через `/exec`. Фейковые записи для C3 удобно лить `smoke.sh` в цикле.

## Unity (задача C2)

`UnityWebRequest.Post(url, json, "application/json")`, таймаут 5 с. Apps Script отвечает `302` на
`script.googleusercontent.com` — `UnityWebRequest` следует за редиректом сам. Любая ошибка сети,
таймаут или непустой `error` в ответе → `Leaderboard offline` и локальный рекорд из сейва.
