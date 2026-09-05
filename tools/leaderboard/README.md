# COPYCAT — лидерборд на Google Sheets (задача C1)

Веб‑приложение Apps Script, привязанное к таблице команды. Клиент (`ILeaderboardService`, задача C2)
шлёт `POST` с результатом попытки и получает в ответ топ‑10 и свой ранг одним запросом.

| | |
|---|---|
| Таблица | https://docs.google.com/spreadsheets/d/1a9t4uwVDtyPE-O1zC9yAPIs1J1wGLTzbJEE7Zz-PF6o/edit |
| Лист | `scores`, шапка `name \| answers \| timeSeconds \| grade \| dateUtc` |
| Скрипт | [`Code.gs`](Code.gs) — единственный источник правды, правим здесь и копируем в Apps Script |
| URL деплоя | **TODO: вписать сюда `/exec`‑URL после деплоя** (нужен для `LeaderboardConfig` в C2) |

## Деплой (делает владелец таблицы: Коля или Егор)

1. Открыть таблицу → **Extensions → Apps Script**.
2. Удалить содержимое `Code.gs` в редакторе, вставить целиком содержимое [`Code.gs`](Code.gs) из репозитория. Сохранить.
3. Выбрать в выпадающем списке функцию `setup`, нажать **Run**, выдать разрешения. Появится лист `scores` с шапкой.
4. **Deploy → New deployment → Web app**:
   - *Execute as* — **Me**;
   - *Who has access* — **Anyone** (именно `Anyone`, не `Anyone with Google account` — иначе игра получит HTML логина вместо JSON).
5. Скопировать URL вида `https://script.google.com/macros/s/<id>/exec` — вписать в таблицу выше, в `docs/PROJECT.md §6.1` и скинуть в чат для C2.
6. Проверить: `./smoke.sh "<url>"` — четыре запроса, ответом всегда JSON. Строки `smoke test` потом удалить из листа.

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
