# CI and Reporting

## Overview

This project uses a single workflow (`.github/workflows/test-and-report.yml`) with branching behavior:

- `dev` branch:
  - Runs fast suite: Unit + Integration + API-Smoke
  - Uploads Allure raw results as build artifacts

- `main` branch:
  - Runs full regression including UI tests
  - Exports coverage (opencover/cobertura)
  - Generates Allure HTML report and publishes it to GitHub Pages (`gh-pages` branch)

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
- On `main`, the workflow builds an Allure HTML report and publishes it to GitHub Pages.
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

