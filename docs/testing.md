# Testing

This repo uses a layered NUnit test suite (Unit / Integration / API / UI) with Allure reporting and coverage collection in CI.

## Test Project Structure

All tests live in `SocksShoppingStore.Tests`:

- `Unit/` — controller/service/model unit tests.
- `Integration/` — integration tests, typically via `WebApplicationFactory<Program>`.
- `API/` — API “smoke” tests for `/api/*` routes.
- `UI/` + `PageObjects/` — Selenium UI smoke tests.

## Test Settings

Tests read settings from (in order of precedence):

- Environment variables
- `SocksShoppingStore.Tests/appsettings.Test.json` (copied to the test output directory)

Supported keys:

- `BASE_URL` / `BaseUrl` — base URL for UI/API tests (e.g., `http://127.0.0.1:5123`)
- `RUN_UI_TESTS` / `RunUi` — enable UI tests (`1/true`)
- `IGNORE_CERT_ERRORS` / `IgnoreCertErrors` — allow insecure localhost only when explicitly enabled
- `USE_TEST_FACTORY` / `UseTestFactory` — run Integration/API against in-memory host via `WebApplicationFactory<Program>`

## Running Tests Locally

### Fast (no external server)

Integration/API tests can run without starting the app when `USE_TEST_FACTORY=1`.

- PowerShell:
  - `setx USE_TEST_FACTORY 1`
  - `dotnet test SocksShoppingStore.Tests/SocksShoppingStore.Tests.csproj -c Release --filter "TestCategory=Unit|TestCategory=Integration|TestCategory=API-Smoke"`

### UI Smoke (requires a running server)

UI tests require the application to be running and reachable by `BASE_URL`.

- Start the app:
  - `dotnet run --project SocksShoppingStore/SocksShoppingStore.csproj --launch-profile https`
- Enable UI tests:
  - `setx RUN_UI_TESTS 1`
- Run UI smoke:
  - `dotnet test SocksShoppingStore.Tests/SocksShoppingStore.Tests.csproj -c Release --filter "TestCategory=UI-Smoke"`

If you run the server over HTTP, set:

- `setx BASE_URL http://127.0.0.1:5123`
- `setx IGNORE_CERT_ERRORS 0`
- `setx USE_TEST_FACTORY 0`

## Categories and Conventions

Categories used across the suite:

- `Unit`
- `Integration`
- `API-Smoke`
- `UI-Smoke`

Allure conventions:

- Test classes declare suite labels (e.g., `[AllureSuite("Unit")]`, `[AllureSuite("UI")]`).
- Each test includes an English `[AllureDescription(...)]` describing What/Steps/Expected.
- UI failures attach a screenshot (see `SocksShoppingStore.Tests/BaseTest.cs`).

## CI Workflow

Workflow file: `.github/workflows/test-and-report.yml`.

### `dev` branch

- Runs fast suite: `Unit + Integration + API-Smoke`.
- Runs scoped coverage for Integration and API.
- Uploads Allure raw results and coverage artifacts.

### `main` branch and Pull Requests

- Starts the app in **HTTP** mode on `http://127.0.0.1:5123` with relaxed limits:
  - `FreeTier__Enabled=false`
  - `RateLimiting__GlobalPerMinute=1000`
  - `RateLimiting__ApiPerMinute=1000`
  - `HttpsRedirect__Enabled=false`
- Runs non-UI tests with coverage (`coverlet.runsettings`).
- Runs `UI-Smoke` separately (no coverage).
- Aggregates `**/allure-results` into a single Allure results folder.
- Generates:
  - Allure HTML report
  - Coverage HTML report (ReportGenerator)
- Publishes reports to GitHub Pages (branch `gh-pages`).

### Manual UI run

Workflow supports `workflow_dispatch` input `run_ui=true` to run UI smoke.

## Coverage Collection

Coverage runsettings files live at repo root:

- `coverlet.runsettings`
- `coverlet.integration.runsettings`
- `coverlet.api.runsettings`

In CI, coverage is generated for non-UI tests by default. There is an additional CI step that can instrument the running app with `coverlet.console` to collect “UI coverage”.

## UI Stability Notes

- UI tests assume the cookie consent banner may appear and will click `#cookie-accept` if present.
- Header readiness is detected via `a[aria-label='Cart']`.
- Prefer stable, semantic selectors when changing UI; avoid brittle DOM-dependent selectors.
