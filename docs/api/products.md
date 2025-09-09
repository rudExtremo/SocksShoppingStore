# Products API

Base path: `/api/products`

- `GET /api/products`
  - Description: Returns the full product catalog as JSON.
  - Rate limit: policy `api` (per-minute window, see `appsettings.json` → `RateLimiting.ApiPerMinute`).
  - Response: `200 OK` with array of `Sock`.

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

