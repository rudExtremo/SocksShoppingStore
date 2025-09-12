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

## QA Framework Architectural Evolution

This section outlines the modernization of the testing infrastructure, expanding beyond the current stack to incorporate disciplines essential for a modern QA engineer.

### Enhanced Test Automation and Reporting

*   **Current State:** The project uses NUnit for unit/integration tests, Selenium for UI tests, and Allure Framework for reporting.
*   **Modernization Strategy:**
    *   **Improved Test Structure:** Implement a clear separation between test types (Unit, Integration, API, UI) within the `SocksShoppingStore.Tests` project structure.
    *   **Advanced Allure Features:** Go beyond basic reports by using Allure features like Steps, Attachments (screenshots on failure), History, and Categories to create rich, debuggable, and informative reports.
    *   **Allure Report Hosting on GitHub Pages:** Implement a CI/CD process to automatically generate and publish the Allure report to a public GitHub Pages URL after each test run. This provides free, persistent, and easily shareable storage for test execution history.

### Performance Testing Integration

*   **Goal:** To demonstrate the ability to identify performance bottlenecks even on a resource-constrained platform.
*   **Tool of Choice:** **k6** is recommended. It is lightweight, developer-centric (tests are written in JavaScript), and designed for easy CI/CD integration, making it ideal for staying within the free-tier limits of GitHub Actions.
*   **Integration Strategy:**
    *   Develop a small set of k6 scripts targeting critical APIs (e.g., product listing, search, add to cart).
    *   Integrate a k6 execution step into the `dotnet-ci.yml` workflow.
    *   Define performance thresholds (SLOs) in the k6 scripts (e.g., p(95) response time < 500ms). The CI job will fail if these thresholds are breached, demonstrating proactive performance monitoring.

### Security Testing Integration (DevSecOps)

*   **Goal:** To "shift-left" security testing by integrating automated scans directly into the CI pipeline.
*   **Tool of Choice:** **OWASP ZAP Baseline Scan** via its official GitHub Action (`zaproxy/action-baseline`). This is a Dynamic Application Security Testing (DAST) tool. The baseline scan is chosen over a full scan as it is non-intrusive, significantly faster, and ideal for frequent CI runs.
*   **Integration Strategy:**
    *   Add a new job to the `dotnet-ci.yml` workflow that runs after the application is deployed to a test environment.
    *   The action will scan the target URL and automatically create/update a GitHub Issue with any vulnerabilities found, demonstrating a closed-loop feedback system.
    *   Configure `rules_file_name` to ignore known false positives, showing an understanding of practical security issue triage.

### Application of Artificial Intelligence in QA

*   **Goal:** To demonstrate the practical, value-oriented application of AI in testing.
*   **Practical Applications:**
    *   **Generative AI for Test Data:** Use a Large Language Model (LLM) to generate a diverse `products.json` file with edge cases: long names, special characters, missing image URLs, negative prices, etc.
    *   **AI-Powered Self-Healing Tests:** As a research task, investigate libraries or write custom C# code that, upon a `NoSuchElementException` in Selenium, uses AI principles to find alternative locators for the element (e.g., by analyzing nearby elements, text content, or attributes).
    *   **AI-Based Visual Testing:** Integrate a tool that uses AI for visual regression testing, intelligently ignoring minor rendering differences and flagging only significant visual bugs.
