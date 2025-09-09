# Overview

SocksShoppingStore is a small, self-contained ASP.NET Core MVC demo that showcases:

- Browsing a simple product catalog of “job-themed” socks (navbar search dropdown, compact filters, lazy load)
- Viewing product details and adding items to a session-backed shopping cart
- EN/RU localization across UI and product texts
- Production-minded concerns: CSP/secure headers, free-tier guard, rate limiting, concurrency limiting, and minimal metrics

The project is intentionally simple, with an in-memory product repository and no database.

## Key Features

- Session cart: Items persisted for the browser session
- Navbar search: magnifier toggles a compact search panel
- Filters: Price min–max, Sort buttons with icons
- Lazy load: client fetches next page via `/api/products` (replaces pagination)
- API endpoint: `GET /api/products` (limited separately)
- Health check: `GET /healthz` returns `OK`
- Operational metrics: `GET /_status` (IP allowlist)
- Localization switch: via cookie or query string (`?culture=en|ru`)

## What’s in Scope

- MVC controllers and Razor views
- Middleware for security headers, free-tier guard, and concurrency caps
- Request rate limiting (global + API policy)
- Minimal config via `appsettings.json`

## What’s Not in Scope

- Real payment or checkout flow
- Database or external storage
- AuthN/AuthZ beyond safe defaults
