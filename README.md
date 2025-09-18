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

### Local Scripts (Windows/PowerShell)

- Start app with health check (non-blocking):
  - `./scripts/start-app.ps1 -Url http://127.0.0.1:5123 -Configuration Release`
  - Logs: `.logs/app-local.log` (stdout), `.logs/app-local.err.log` (stderr), PID: `.logs/app.pid`
- Stop app (uses PID file, with fallback scan):
  - `./scripts/stop-app.ps1` (add `-Force` if needed)
- Run tests like CI:
  - Fast (dev): `./scripts/run-tests.ps1 -Suite dev-fast`
  - Full (main): `./scripts/run-tests.ps1 -Suite main-full -BaseUrl http://127.0.0.1:5123`
  - UI smoke: `./scripts/run-tests.ps1 -Suite ui-smoke -BaseUrl http://127.0.0.1:5123`
  - Results: `.logs/TestResults/<Suite>`, console log: `.logs/test-<Suite>.log`

For Russian-language docs, see `README.ru.md` and `docs/*/*.ru.md` counterparts.

## License

See `LICENSE`.
## CI / Reports

- CI Workflows: `.github/workflows/test-and-report.yml`
- Branches:
  - `dev`: fast tests (Unit + Integration + API-Smoke), Allure artifacts
  - `main`: full regression (incl. UI), coverage export, Allure report published to GitHub Pages
- Allure Report (GitHub Pages): https://rudExtremo.github.io/SocksShoppingStore (first publish occurs after a successful main run)

Badges:

![CI Dev](https://github.com/rudExtremo/SocksShoppingStore/actions/workflows/test-and-report.yml/badge.svg?branch=dev)
![CI Main](https://github.com/rudExtremo/SocksShoppingStore/actions/workflows/test-and-report.yml/badge.svg?branch=main)

