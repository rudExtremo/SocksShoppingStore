# Setup

## Requirements

- .NET SDK 8.0+

## Local Run

- From repo root: `dotnet run --project SocksShoppingStore`
- Open the printed HTTPS URL (e.g., `https://localhost:5001`)
- Health probe: `GET /healthz`

### Local Scripts (Windows/PowerShell)

- Start app: `./scripts/start-app.ps1 -Url http://127.0.0.1:5123 -Configuration Release`
- Stop app: `./scripts/stop-app.ps1`
- Fast tests (dev): `./scripts/run-tests.ps1 -Suite dev-fast`
- Full tests (main-like): `./scripts/run-tests.ps1 -Suite main-full -BaseUrl http://127.0.0.1:5123`
- UI smoke: `./scripts/run-tests.ps1 -Suite ui-smoke -BaseUrl http://127.0.0.1:5123`

## Configuration

Edit `SocksShoppingStore/appsettings.json`:

- `FreeTier`: enable guard and allowlist paths
- `RateLimiting`: `GlobalPerMinute`, `ApiPerMinute`
- `Concurrency`: `MaxConcurrentRequests`
- `Status`: `AllowIPs` for `/_status`, `LatencyWindowSize`
- `Legal`: controller name and contact email

For development overrides, use `appsettings.Development.json`.

## Localization

- Default culture: `en` (`en-US` for numbers)
- Supported UI cultures: `en`, `ru`
- Switch via cookie POST (`/Localization/Set`) or query `?culture=en|ru`

## SSL Note

ASP.NET dev cert is required for HTTPS. If missing, install with:

- `dotnet dev-certs https --trust`
# Setup
## Test Settings (local and CI)

This repo centralizes test configuration to avoid hardcoding URLs and flags in test code.

- File: `SocksShoppingStore.Tests/appsettings.Test.json` is copied to test bin and can define:
  - `BaseUrl`: default UI/API base (e.g., `https://localhost:7068` locally)
  - `RunUi`: enable/disable Selenium UI tests by default
  - `IgnoreCertErrors`: allow insecure localhost only when needed (CI)
  - `UseTestFactory`: use `WebApplicationFactory<Program>` for Integration/API tests (recommended)

- Environment variables override JSON:
  - `BASE_URL` — overrides `BaseUrl`
  - `RUN_UI_TESTS` — `1/true` to enable UI tests
  - `IGNORE_CERT_ERRORS` — `1/true` to ignore TLS errors (CI only)
  - `USE_TEST_FACTORY` — `1/true` to run Integration/API via in-memory test host

Examples (PowerShell, Windows 11):

- Unit + Integration + API (fast), in-memory host:
  - `setx USE_TEST_FACTORY 1`
  - `dotnet test SocksShoppingStore.Tests/SocksShoppingStore.Tests.csproj -c Release --filter "TestCategory=Unit|TestCategory=Integration|TestCategory=API-Smoke"`

- UI smoke locally (dev cert trusted):
  - Ensure: `dotnet dev-certs https --trust`
  - `setx RUN_UI_TESTS 1`
  - `dotnet run --project SocksShoppingStore/SocksShoppingStore.csproj --launch-profile https`
  - `dotnet test SocksShoppingStore.Tests/SocksShoppingStore.Tests.csproj -c Release --filter "TestCategory=UI-Smoke"`

- UI in CI-style (HTTP, no cert):
  - `setx BASE_URL http://127.0.0.1:5123`
  - `setx RUN_UI_TESTS 1`
  - `setx IGNORE_CERT_ERRORS 0`
  - `setx USE_TEST_FACTORY 0`

Notes:
- Integration/API tests do not require an external server when `USE_TEST_FACTORY=1`.
- UI tests always require a running server (the application), pointed by `BASE_URL`.

