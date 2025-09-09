# SocksShoppingStore

A simple ASP.NET Core MVC demo app for browsing fun “job-themed” socks, adding them to a session-backed cart, and exploring basic production-friendly concerns (security headers, rate limiting, concurrency caps, metrics, localization).

- Quick tech overview: ASP.NET Core 8 MVC, Razor Views, in-memory product repository, session cart, EN/RU localization.
- Safety: CSP and secure headers, free-tier guard, global and API rate limits, simple concurrency limiter.
- Operability: health endpoint, lightweight request metrics, configuration via `appsettings.json`.

## Quick Start

- Prerequisites: .NET SDK 8.0+
- Run: `dotnet run --project SocksShoppingStore`
- Open: `https://localhost:5001` (or the URL from the console)
- Health: `GET /healthz` → `OK`

## Repository Map

- `SocksShoppingStore/Controllers` — MVC controllers (`Home`, `Products`, `ProductsApi`, `Cart`, `Legal`, `Localization`).
- `SocksShoppingStore/Middleware` — `SecurityHeaders`, `FreeTierGuard`, `ConcurrencyLimiter`.
- `SocksShoppingStore/Services` — `ProductCatalogLocalizer`, request `RateOptions`, basic `RequestMetrics`.
- `SocksShoppingStore/Data` — in-memory `ProductRepository`.
- `SocksShoppingStore/Models` — `Sock`, `ShoppingCart`, `CartItem`, `ErrorViewModel`.
- `SocksShoppingStore/Views` — Razor Pages (UI), `wwwroot` for static assets.
- `SocksShoppingStore/Program.cs` — pipeline, DI, localization, sessions, rate limiting, metrics.
- `.codex` — legacy RTF/PDF roadmap and description (migrated into `docs/`).

## Documentation

- Overview: `docs/overview.md`
- Architecture: `docs/architecture.md`
- Setup: `docs/setup.md`
- API: `docs/api/products.md`
- Internationalization: `docs/internationalization.md`
- Security & Limits: `docs/security-and-limits.md`
- Metrics: `docs/metrics.md`
- Roadmap: `docs/roadmap.md`
- Contributing: `docs/contributing.md`

For Russian-language docs, see `README.ru.md` and `docs/*/*.ru.md` counterparts.

## License

See `LICENSE`.

