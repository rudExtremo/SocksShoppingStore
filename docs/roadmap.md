# Feature Expansion Roadmap

This section describes a phased approach to application development. The choice of features is based on an analysis of modern e-commerce practices and is adapted for implementation with minimal resource costs, while providing rich and diverse testing scenarios. Each new feature is intentionally introduced not for its commercial value, but for the testing opportunities it opens up, which directly corresponds to the main goal of the project.

## Phase 1: Fundamental User Experience and Data Persistence

**Goal:** To move from an anonymous user experience to a state where user data is saved and user-generated content is created.

**Proposed Features:**

*   **User Accounts (Registration and Login):** Implementation of a basic authentication system. This is a prerequisite for all subsequent personalization features.
    *   *Testing Opportunities:* Verifying authentication workflows, session management, password policy testing, input validation (SQL injection, XSS), error handling for incorrect credentials.
*   **Order History:** Allowing authenticated users to view their past orders.
    *   *Testing Opportunities:* Testing data retrieval and display for authenticated users, data integrity checks, access control testing (user A cannot see user B's orders).
*   **Product Reviews and Ratings:** Allowing users to leave text reviews and star ratings for products.
    *   *Testing Opportunities:* Form submission testing, user-generated content validation, XSS vulnerability testing, interaction with the database (JSON file), UI updates after submission.
*   **Wishlist:** Allows users to save products for future purchases.
    *   *Testing Opportunities:* State management, data persistence between sessions, UI/UX for adding/removing items.

## Phase 2: Advanced Product Discovery and Engagement

**Goal:** To improve the ways users find and interact with products by introducing more complex logic.

**Proposed Features:**

*   **Faceted Search and Filtering:** Implementation of server-side filtering by attributes such as color, size, and price range.
    *   *Testing Opportunities:* Complex API/query testing, load testing the JSON-file "database", UI state management, validation of filter combination logic.
*   **Promo Code System:** A simple system for applying discount codes in the cart.
    *   *Testing Opportunities:* Business logic validation (correct discount, expiration dates, usage limits), cart total recalculation, error handling for invalid codes, negative testing (applying a used code).
*   **Basic AI Recommendations (Mock):** Instead of a heavy ML model, implement a simple, logic-based recommendation system, e.g., "Frequently bought with this item...", based on static rules or simple data analysis.
    *   *Testing Opportunities:* Validation of recommendation logic, API testing of the recommendation endpoint, checking for performance degradation on product pages.

## Phase 3: Transaction and Post-Purchase Optimization

**Goal:** To build a more complete and realistic end-to-end purchasing process.

**Proposed Features:**

*   **Guest Checkout:** Allows users to make purchases without creating an account.
    *   *Testing Opportunities:* Testing parallel user flows (guest vs. registered user), handling of temporary user information.
*   **Mock Payment Gateway Integration:** Simulating interaction with a payment provider (e.g., Stripe or PayPal), handling successful, failed, and pending states.
    *   *Testing Opportunities:* End-to-end testing of the transactional flow, error handling and recovery, order state management, testing with API mocks.
*   **Automatic Order Confirmation (Mock):** Simulating the sending of an order confirmation email after a successful purchase.
    *   *Testing Opportunities:* Post-transaction trigger testing, integration with a mock email service, validation of email content.

## Summary Table: Roadmap and Testing Opportunities

| Phase | Feature                      | Priority | Description                                           | Key Testing Opportunities                               |
| :---- | :--------------------------- | :------- | :---------------------------------------------------- | :------------------------------------------------------ |
| 1     | User Accounts                | High     | User registration, login, and logout.                 | Auth, session management, input validation, security.   |
| 1     | Order History                | High     | View past orders for authenticated users.             | Access control, data integrity, UI/UX.                  |
| 1     | Reviews and Ratings          | Medium   | Users can leave reviews and ratings.                  | Form validation, UGC security (XSS), DB interaction.    |
| 1     | Wishlist                     | Medium   | Save items for future purchases.                      | State management, data persistence.                     |
| 2     | Faceted Search & Filtering   | High     | Filter catalog by attributes (price, color, size).    | Complex API testing, load testing, logic validation.    |
| 2     | Promo Code System            | Medium   | Apply discount codes in the cart.                     | Business logic testing, error handling, negative cases. |
| 2     | Basic AI Recommendations (Mock) | Low      | Display related products based on simple rules.       | Logic validation, API testing, performance checks.      |
| 3     | Guest Checkout               | Medium   | Purchase without creating an account.                 | Parallel user flows, temporary data handling.           |
| 3     | Mock Payment Gateway         | High     | Simulate successful and failed payments.              | E2E testing, error handling, order state management.    |
| 3     | Mock Order Confirmation      | Medium   | Simulate sending a confirmation email.                | Post-transaction triggers, integration testing.         |