# Шрифты D1

| Файл | Источник | Лицензия |
|---|---|---|
| `LuckiestGuy.ttf` | Копия уже используемого `Assets/AddressableResources/Content/UI/Fonts/LuckiestGuy.ttf` | Apache 2.0, `LuckiestGuy-LICENSE.txt` |
| `PatrickHand-Regular.ttf` | [Google Fonts / Patrick Hand](https://github.com/google/fonts/tree/main/ofl/patrickhand) | SIL OFL 1.1, `PatrickHand-OFL.txt` |
| `Nunito.ttf` | [Google Fonts / Nunito](https://github.com/google/fonts/tree/main/ofl/nunito), исходный variable font | SIL OFL 1.1, `Nunito-OFL.txt` |
| `Nunito-Bold.ttf` | Статический экземпляр `Nunito.ttf` с `wght=700` | SIL OFL 1.1, `Nunito-OFL.txt`; изменение: ось веса зафиксирована для resvg |

Получено 05.09.2026. Полные тексты лицензий сохранены рядом. Luckiest Guy распространяется по Apache 2.0, а не OFL; это уточнение к первоначальной записи D-16.

Статический Nunito нужен для одинакового веса в resvg и предпросмотре; исходный variable font сохранён. Для повторного получения экземпляра (FontTools 4.63.0):

```sh
fonttools varLib.instancer Nunito.ttf wght=700 --output Nunito-Bold.ttf
```

Обычная сборка использует готовые TTF и не требует FontTools. Для Unity TMP-ассеты создаются позднее в рамках интеграции арта, без системных шрифтов macOS.
