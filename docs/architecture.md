# Architecture

This is a classic ASP.NET Core MVC app structured by responsibility:

- Controllers (`SocksShoppingStore/Controllers`): HTTP endpoints and UI orchestration
- Middleware (`SocksShoppingStore/Middleware`): cross‑cutting pipeline concerns
- Services (`SocksShoppingStore/Services`): localization helpers and options
- Data (`SocksShoppingStore/Data`): in‑memory product source
- Models (`SocksShoppingStore/Models`): domain and view models
- Views (`SocksShoppingStore/Views`): Razor UI; `wwwroot` for static assets

## Request Pipeline (Program.cs)

- HTTPS redirection and static files with cache headers (7 days)
- Request localization (EN/RU) via query string and cookie providers
- Free‑tier guard middleware to avoid accidental costs/deploy exposure
- Security headers middleware (CSP, frame, sniffing, referrer, permissions)
- Cookie policy (Lax SameSite, HttpOnly, Secure based on environment)
- Concurrency limiter middleware (in‑process cap, fast‑fail on saturation)
- Rate limiter (global default + named `api` policy) with friendly 429 body
- Session and MVC routing
- Operational endpoints: `GET /healthz`, `GET /robots.txt`, `GET /_status`

## Controllers

- `HomeController`: catalog listing with optional `q` search; localized texts
- `ProductsController`: product details (`/Products/Details?id=`)
- `ProductsApiController` (`[Route("api/products")]`): returns all products as JSON; `[EnableRateLimiting("api")]`
- `CartController`: session‑backed cart (add/remove/delete/clear)
- `LegalController`: static legal pages (TOS/Privacy)
- `LocalizationController`: POST culture switch with safe local redirect

## UI Notes

- Navbar search: compact dropdown near the language selector
- Catalog filters: Price (min–max), Sort (buttons with icons)
- Lazy load: a “Load more” button fetches `/api/products?page=N&pageSize=M` and appends cards client‑side (no infinite scroll)

## Middleware

- `SecurityHeadersMiddleware`: sets CSP with per‑request nonce, denies framing, `nosniff`, strict referrer and permissions
- `FreeTierGuardMiddleware`: if enabled and not allowlisted path, returns 503 (blocks on Azure or full site per config)
- `ConcurrencyLimiterMiddleware`: bounding concurrent requests with `SemaphoreSlim`, returns 429 when saturated

## Services and Data

- `ProductCatalogLocalizer`: replaces product names/descriptions per culture (EN default, RU mapped)
- `IProductRepository`: abstraction for product storage
- `LegacyProductRepository`: wraps legacy in-memory list (default)
- `JsonProductRepository`: optional JSON file storage (configurable path)
- `RequestMetrics`: rolling latency window + counts; exposed via `/_status`

## Configuration

`SocksShoppingStore/appsettings.json` provides:

- `FreeTier`: feature toggle, Azure detection, allowlist paths
- `RateLimiting`: `GlobalPerMinute`, `ApiPerMinute`
- `Concurrency`: `MaxConcurrentRequests`
- `Status`: `AllowIPs`, `LatencyWindowSize`
- `Legal`: controller name and contact email
- `Repository`: provider `InMemory|Json` and JSON path
