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

- `SocksShoppingStore/Controllers` — MVC controllers (`Home`, `Products`, `ProductsApi`, `Cart`, `Checkout`, `StripeWebhook`, `Legal`, `Localization`).
- `SocksShoppingStore/Middleware` — `SecurityHeaders`, `FreeTierGuard`, `ConcurrencyLimiter`.
- `SocksShoppingStore/Services` — `ProductCatalogLocalizer`, `StripeCheckoutService`, `PaymentSessionStore`.
- `SocksShoppingStore/Options` — options types (e.g., `StripeOptions`).
- `SocksShoppingStore/Data` — repository (`IProductRepository`, `LegacyProductRepository`, optional `JsonProductRepository`).
- `SocksShoppingStore/Models` — `Sock`, `ShoppingCart`, `CartItem`, `ErrorViewModel`.
- `SocksShoppingStore/Views` — Razor Pages (UI), `wwwroot` for static assets.
- `SocksShoppingStore/Program.cs` — pipeline, DI, localization, sessions, rate limiting, metrics.

## Documentation

- Overview: `docs/overview.md`
- Architecture: `docs/architecture.md`
- Setup: `docs/setup.md`

- Testing: `docs/testing.md`
- Agent guide: `docs/SOCKS_AGENT_GUIDE.md`

For Russian-language docs, see `README.ru.md`.

## License

See `LICENSE`.
## CI / Reports

- CI Workflows: `.github/workflows/test-and-report.yml`
- Branches:
  - `dev`: fast tests (Unit + Integration + API-Smoke), Allure artifacts
  - `main`: runs non-UI tests with coverage, runs UI smoke, builds Allure + coverage HTML, publishes to GitHub Pages
- Reports (GitHub Pages): https://rudExtremo.github.io/SocksShoppingStore

Badges:

![CI Dev](https://github.com/rudExtremo/SocksShoppingStore/actions/workflows/test-and-report.yml/badge.svg?branch=dev)
![CI Main](https://github.com/rudExtremo/SocksShoppingStore/actions/workflows/test-and-report.yml/badge.svg?branch=main)

