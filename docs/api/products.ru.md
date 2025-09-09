# API товаров

Базовый путь: `/api/products`

- `GET /api/products`
  - Назначение: возвращает (с учётом фильтров) каталог товаров в JSON.
  - Параметры:
    - `q`: поиск по имени/описанию
    - `sort`: `name_asc|name_desc|price_asc|price_desc` (по умолчанию `name_asc`)
    - `minPrice` / `maxPrice`: фильтры по цене (decimal)
    - `page` / `pageSize`: пагинация (по умолчанию `1`/`10`)
    - `culture`: `en|ru` (по умолчанию определяется)
  - Лимит: политика `api` (поминутное окно, см. `appsettings.json` → `RateLimiting.ApiPerMinute`).
  - Ответ: `200 OK` с массивом `Sock`.
  - Заголовки:
    - `X-Total-Count`: общее число элементов до пагинации
    - `ETag` и `Last-Modified`; поддержка `If-None-Match` / `If-Modified-Since` → `304`
    - `Cache-Control: public, max-age=60`

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

- `GET /api/products/{id}`
  - Назначение: возвращает товар по идентификатору
  - Параметр: `culture` опционально (`en|ru`)
  - Ответ: `200 OK` с `Sock` или `404`, если не найден
  - Кэширование: `ETag`, `Last-Modified`, `If-None-Match`, `If-Modified-Since`
