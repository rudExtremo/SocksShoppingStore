# API товаров

Базовый путь: `/api/products`

- `GET /api/products`
  - Назначение: возвращает полный каталог товаров в JSON.
  - Лимит: политика `api` (поминутное окно, см. `appsettings.json` → `RateLimiting.ApiPerMinute`).
  - Ответ: `200 OK` с массивом `Sock`.

## Модель Sock (JSON)

- `id` (number)
- `name` (string)
- `description` (string)
- `price` (number, decimal)
- `imageUrl` (string)

Пример:

```json
[
  {
    "id": 1,
    "name": "Coder's Comfort",
    "description": "Breathable cotton crew socks for long coding sessions.",
    "price": 3.50,
    "imageUrl": "/images/coder_socks.png"
  }
]
```

