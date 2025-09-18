# Test Conventions

This document standardizes test names, metadata, and authoring patterns across Unit, Integration, API, and UI tests.

## Naming

- Pattern: `Area_Action_Result`
  - Examples:
    - `Cart_AddSingleItem_UpdatesHeaderCounter`
    - `ProductsApi_GetAll_SetsCachingHeaders`
    - `Middleware_ConcurrencyLimiter_Returns429WhenSaturated`
- Keep names concise, assert intent, and avoid acronyms that obscure meaning.

## Categories

- By layer: `Unit`, `Integration`, `API-Smoke`, `UI-Smoke` (extend with `UI-Regression` if needed)
- By intent (optional): `Positive`, `Negative`, `Security`, `Performance`, `Accessibility`

## Required Allure Metadata

Each test MUST include an English description with What/Steps/Expected.

- Attribute: `[AllureDescription(@"What: ...\nSteps: ...\nExpected: ...")]`
- All test classes MUST declare an Allure Suite at class level to avoid scattering in the report:
  - Unit: `[AllureSuite("Unit")]`
  - Integration (incl. API): `[AllureSuite("Integration")]`
  - UI: `[AllureSuite("UI")]`
  - Add `[AllureEpic("SocksShoppingStore")]` to keep reports consistent.

- UI test classes additionally SHOULD declare:
  - `[AllureEpic("SocksShoppingStore")]`
  - `[AllureFeature("<Area>")]` e.g., `Catalog`, `Cart`, `Checkout`, `Header`, `Product`

Example:

```csharp
[Test]
[AllureDescription(@"What: Add a single item via Home page and verify header counter.
Steps:
1) Open Home.
2) Click Add to Cart on the first item.
3) Observe cart counter in header.
Expected: Counter increments from 0 to 1.")]
public void Cart_AddSingleItem_UpdatesHeaderCounter() { /* ... */ }
```

Recommended optional labels:
- `[AllureEpic("SocksShoppingStore")]`
- `[AllureSuite("UI")]`, `[AllureSuite("API")]`, etc.
- `[AllureStory("Cart")]`, `[AllureFeature("Localization")]`

## UI Authoring Guidelines

- Prefer stable locators (semantic classes, `data-*` attributes) over brittle structures (`nth-child`, deep CSS chains).
- Always wait for navigation or DOM updates before assertions (explicit waits or retry loops with timeouts).
- Keep page objects minimal and focused; expose actions and key assertions.
- Avoid hard sleeps; prefer condition-based waits.

Stability helpers used in the suite:
- Pre-waits: DOM ready + header ready after navigation (see `BaseTest.AcceptCookiesIfPresent`).
- Header cart summary change wait after AddToCart.
- Catalog stabilization before reading prices (`HomePage.WaitForCatalogStable`).
- Price reads via one JS snapshot to avoid stale references.

Parallelism:
- Unit/Integration/API may use multiple workers (runsettings `NumberOfTestWorkers`).
- UI tests are marked `[NonParallelizable]` and run sequentially by fixture.

## Negative Tests Policy

- For scenarios that may require configuration changes or have side effects (e.g., rate limiting, security blocks), first propose the approach and wait for approval before implementation.

## Coverage Collection

- Use `coverlet.runsettings` checked into the repo.
- Run per-category collections with: `--settings coverlet.runsettings`.

