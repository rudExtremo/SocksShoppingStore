# Products API

Base path: `/api/products`

- `GET /api/products`
  - Description: Returns the (optionally filtered) product catalog as JSON.
  - Query params:
    - `q`: search by name/description
    - `sort`: `name_asc|name_desc|price_asc|price_desc` (default `name_asc`)
    - `minPrice` / `maxPrice`: decimal filters
    - `page` / `pageSize`: paging (defaults `1`/`10`)
    - `culture`: `en|ru` (default inferred)
  - Rate limit: policy `api` (per-minute window, see `appsettings.json` → `RateLimiting.ApiPerMinute`).
  - Response: `200 OK` with array of `Sock`.
  - Headers:
    - `X-Total-Count`: total items before paging
    - `ETag` and `Last-Modified`; supports `If-None-Match` / `If-Modified-Since` → `304`
    - `Cache-Control: public, max-age=60`

## Sock model (JSON)

- `id` (number)
- `name` (string)
- `description` (string)
- `price` (number, decimal)
- `imageUrl` (string)

Example:

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
  - Description: Returns a single product by id
  - Query: optional `culture=en|ru`
  - Response: `200 OK` with `Sock` or `404` if not found
  - Caching: `ETag`, `Last-Modified`, `If-None-Match`, `If-Modified-Since`
