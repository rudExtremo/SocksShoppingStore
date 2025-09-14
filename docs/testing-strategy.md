# Overall Test Plan for "SocksShoppingStore"

This section provides a comprehensive list of test scenarios for the enhanced application, based on e-commerce testing best practices.

## Test Strategy and Coverage

*   **Testing Levels:**
    *   **Unit Tests (NUnit):** Focus on individual methods in controllers, services, and models.
    *   **Integration Tests (NUnit):** Test the interaction between components, especially the repository's interaction with the `products.json` file.
    *   **API Tests:** Directly test the RESTful API for correctness, performance, and security.
*   **End-to-End (UI) Tests (Selenium):** Simulate full user scenarios through the web interface.
*   **Execution Modes:**
    *   Local fast runs: Unit + Integration/API via `WebApplicationFactory` (no external server required)
    *   Local UI runs: start app locally (`https` profile), run Selenium against `BASE_URL`
    *   CI dev branch: fast suite (Unit + Integration + API-Smoke), Allure results archived
    *   CI main branch: full regression (incl. UI), coverage export, Allure published to GitHub Pages
*   **Coverage Goals:** High code coverage (>80%) for unit/integration tests. For UI/API tests, the focus is on requirements coverage, ensuring every user story and feature is tested.

## Positive Test Scenarios ("Happy Path")

*   **Account Management:**
    *   Verify successful user registration with valid data.
    *   Verify successful login with correct credentials.
    *   Verify that a user can successfully log out.
*   **Product Discovery:**
    *   Verify that the catalog page loads and displays products correctly.
    *   Verify that searching for a valid product name returns relevant results.
    *   Verify that filtering by category/price works as expected.
*   **Shopping Cart:**
    *   Verify that a user can add an item to the cart from the catalog and product pages.
    *   Verify that a user can increase/decrease the quantity of an item in the cart.
    *   Verify that a user can remove an item from the cart.
    *   Verify that the cart total is updated correctly after changes.
*   **Checkout and Payment:**
    *   Verify that a registered user can complete the checkout process.
    *   Verify that a guest user can complete the checkout process.
    *   Verify that a valid promo code applies the correct discount.
    *   Verify a successful transaction through the mock payment gateway.
    *   Verify that an order is created and visible in "Order History" after successful payment.

## Negative and Edge Case Scenarios ("Unhappy Path")

*   **Account Management:**
    *   Attempt to register with an already existing email.
    *   Attempt to register with an invalid email format or a weak password.
    *   Attempt to log in with an incorrect password/username.
*   **Product Discovery:**
    *   Search for a product that does not exist.
    *   Search using special characters or SQL injection strings (e.g., `' OR 1=1;--`).
    *   Apply conflicting or invalid filter combinations.
*   **Shopping Cart:**
    *   Attempt to add more units of a product than are in stock (requires inventory functionality).
    *   Attempt to update the quantity to a negative number or zero.
*   **Checkout and Payment:**
    *   Attempt to apply an invalid or expired promo code.
    *   Simulate a failed payment from the mock gateway and verify that the order is not created.
    *   Attempt to check out with an empty cart.
    *   Submit the shipping address form with missing or invalid data.

## Non-Functional Test Scenarios

*   **Performance:**
    *   (k6) Measure the API response time for key endpoints under a simulated load of 10 virtual users.
    *   (Browser DevTools) Measure front-end metrics: Page Load Time, LCP, and CLS.
*   **Security:**
    *   (OWASP ZAP) Run a baseline scan to check for common vulnerabilities like missing security headers, XSS, etc.
    *   Manually check for insecure direct object references (e.g., trying to access `/orders/123` without being the owner of order 123).
*   **Usability and Accessibility:**
    *   Verify that the site can be navigated using only the keyboard (Tab, Enter, Space).
    *   Verify that all images have descriptive alt text.
    *   Check for sufficient color contrast using browser tools.
*   **Compatibility:**
    *   Test key user scenarios on the latest versions of Chrome, Firefox, and Edge on desktop.
    *   Use browser emulation to test the responsive design on popular mobile resolutions (e.g., iPhone 12, Samsung Galaxy S21).

## Test Coverage Matrix

## CI/CD Flow and Allure Reporting

- GitHub Actions workflow: `.github/workflows/test-and-report.yml`
- Branch `dev`:
  - Runs Unit, Integration, API-Smoke only (fast feedback)
  - Uploads Allure results as artifacts
- Branch `main`:
  - Runs full regression incl. UI
  - Generates Allure report and publishes to GitHub Pages (branch `gh-pages`)
- Manual UI run:
  - Trigger `workflow_dispatch` with `run_ui=true` to validate new UI tests independently of default dev pipeline

## Test Categories and Filters

Common categories used to slice runs in CI and locally:

- By layer: `Unit`, `Integration`, `API-Smoke`, `UI-Smoke` (extend with `UI-Reg` if needed)
- By intent: `Positive`, `Negative`, `Security`, `Performance`, `Accessibility`

Examples:
- `--filter "TestCategory=Unit|TestCategory=Integration|TestCategory=API-Smoke"`
- `--filter "TestCategory=UI-Smoke"`

## Centralized Test Settings

- File: `SocksShoppingStore.Tests/appsettings.Test.json` with keys:
  - `BaseUrl`, `RunUi`, `IgnoreCertErrors`, `UseTestFactory`
- Environment overrides: `BASE_URL`, `RUN_UI_TESTS`, `IGNORE_CERT_ERRORS`, `USE_TEST_FACTORY`
- Integration/API default to `WebApplicationFactory<Program>` when `UseTestFactory=true` (recommended for speed/stability)

## UI/UX Notes for Tests

- Add to Cart UX: stays on the current page by default.
  - Controller accepts `returnUrl` (preferred), otherwise redirects back to validated `Referer`.
  - UI tests navigate explicitly to the Cart when needed.

This matrix visualizes the overall testing strategy, showing which test types provide primary coverage for each feature.

| Feature / User Story          | Unit | Integration | API  | UI (E2E) | Performance | Security |
| :---------------------------- | :--: | :---------: | :--: | :------: | :---------: | :------: |
| User Registration/Login       |  ✓   |      ✓      |  ✓   |    ✓     |             |    ✓     |
| Product Catalog Viewing       |      |      ✓      |  ✓   |    ✓     |      ✓      |          |
| Product Search & Filtering    |  ✓   |      ✓      |  ✓   |    ✓     |      ✓      |    ✓     |
| Add to Cart                   |  ✓   |             |  ✓   |    ✓     |             |          |
| Cart Management (Qty, Remove) |  ✓   |             |  ✓   |    ✓     |             |          |
| Apply Promo Code              |  ✓   |      ✓      |  ✓   |    ✓     |             |          |
| Checkout Process              |  ✓   |             |  ✓   |    ✓     |             |    ✓     |
| View Order History            |      |      ✓      |  ✓   |    ✓     |             |    ✓     |
| Add Product Review            |  ✓   |      ✓      |  ✓   |    ✓     |             |    ✓     |
