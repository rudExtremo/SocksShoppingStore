# Чекаут (Демо)

Демо‑поток без оплаты: сбор адреса доставки, обзор заказа и финализация в пределах сессии.

- Маршруты:
  - `GET /Checkout` — форма доставки
  - `POST /Checkout` — валидация и сбор черновика (сессия `OrderDraft`)
  - `GET /Checkout/Review` — сводка позиций и итога
  - `POST /Checkout/Confirm` — финализация; очистка корзины; сохранение `LastOrder`
  - `GET /Checkout/ThankYou` — страница «спасибо»

- Валидация:
  - DataAnnotations на полях; клиентская unobtrusive‑валидация
  - Honeypot `Website` (скрытое поле) должен оставаться пустым
  - Anti-forgery на POST

- Хранение:
  - Внешней персистентности нет; заказы живут в сессии (`OrderDraft`/`LastOrder`)

- Логирование:
  - `checkout_start` и `checkout_confirmed` (количество позиций/итог)

- Ограничения:
  - Нет оплаты, нет email; подписи — EN по умолчанию

