# Checkout (Demo)

A payment-less, demo checkout flow that collects shipping details, reviews the order, and finalizes it in-session.

- Routes:
  - `GET /Checkout` — shipping form
  - `POST /Checkout` — validate + build draft order (session `OrderDraft`)
  - `GET /Checkout/Review` — summary of items and totals
  - `POST /Checkout/Confirm` — finalize; clears cart; stores `LastOrder` for Thank You
  - `GET /Checkout/ThankYou` — confirmation page

- Validation:
  - DataAnnotations on fields; client-side unobtrusive enabled by partial
  - Honeypot `Website` hidden field must be empty
  - Anti-forgery on POSTs

- Storage:
  - No external persistence; orders live in session (`OrderDraft`/`LastOrder`)

- Logging:
  - Logs `checkout_start` and `checkout_confirmed` with item count / totals

- Limitations:
  - No payments; no email; only EN labels by default

