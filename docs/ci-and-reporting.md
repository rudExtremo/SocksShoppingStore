# CI and Reporting

## Overview

This project uses a single workflow (`.github/workflows/test-and-report.yml`) with branching behavior:

- `dev` branch:
  - Runs fast suite: Unit + Integration + API-Smoke
  - Uploads Allure raw results as build artifacts

- `main` branch:
  - Starts the app in Dev env with relaxed limits for stable UI runs
  - Runs non-UI tests with coverage (Unit + Integration + API-Smoke)
  - Runs UI-Smoke separately (no coverage), then optional UI coverage (instrumented)
  - Aggregates all `**/allure-results`, builds Allure HTML report, and publishes it to GitHub Pages (`gh-pages` branch)

- Manual UI run:
  - Trigger the workflow via `workflow_dispatch` with `run_ui=true` to validate new UI tests without impacting the default dev pipeline.

## Test Settings

Centralized test settings live in `SocksShoppingStore.Tests/appsettings.Test.json` and can be overridden via environment variables:

- `BASE_URL` — base URL for UI/API tests
- `RUN_UI_TESTS` — enable/disable Selenium UI tests
- `IGNORE_CERT_ERRORS` — allow insecure localhost (CI only)
- `USE_TEST_FACTORY` — use `WebApplicationFactory<Program>` for Integration/API

## Allure

- Raw results are collected under `allure-results` (see `SocksShoppingStore.Tests/allureConfig.json`).
- On `main`, the workflow aggregates all per-run `**/allure-results` into a single folder before report generation.
- URL: https://rudExtremo.github.io/SocksShoppingStore

## Local Tips

- Fast suite (no external server required):
  - `setx USE_TEST_FACTORY 1`
  - `dotnet test SocksShoppingStore.Tests/SocksShoppingStore.Tests.csproj -c Release --filter "TestCategory=Unit|TestCategory=Integration|TestCategory=API-Smoke"`
  - Scope mapping: Allure Severity → Smoke=critical, Regression=normal

- UI locally (trusted dev cert):
  - `dotnet dev-certs https --trust`
  - `setx RUN_UI_TESTS 1`
  - `dotnet run --project SocksShoppingStore/SocksShoppingStore.csproj --launch-profile https`
  - `dotnet test SocksShoppingStore.Tests/SocksShoppingStore.Tests.csproj -c Release --filter "TestCategory=UI-Smoke"`


### Local Scripts (Windows/PowerShell)

- Start app with health check: `./scripts/start-app.ps1 -Url http://127.0.0.1:5123 -Configuration Release`
- Stop app: `./scripts/stop-app.ps1`
- Dev fast tests: `./scripts/run-tests.ps1 -Suite dev-fast`
- Main-like tests: `./scripts/run-tests.ps1 -Suite main-full -BaseUrl http://127.0.0.1:5123`
- UI smoke: `./scripts/run-tests.ps1 -Suite ui-smoke -BaseUrl http://127.0.0.1:5123`

Results are written under `.logs/TestResults/<Suite>`, and console output is captured to `.logs/test-<Suite>.log`.

### Notes for UI Stability (Local/CI)

- Prefer running the app with relaxed limits during UI runs:
  - Local script `./scripts/start-app.ps1` uses `-TestMode` (default) to set:
    - `ASPNETCORE_ENVIRONMENT=Development`, `RateLimiting__GlobalPerMinute=1000`, `RateLimiting__ApiPerMinute=1000`, `FreeTier__Enabled=false`
- In CI (main), the Start app step exports the same env.
