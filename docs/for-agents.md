# For AI Agents

A quick reference to programmatic entry points, limits, and safe operations.

## Entry Points

- Browse catalog: `GET /` (HTML)
- Product details: `GET /Products/Details?id=<int>` (HTML)
- Products API: `GET /api/products` (JSON)
- Health: `GET /healthz` (text `OK`)
- Metrics: `GET /_status` (JSON; IP allowlist)

## Limits and Guards

- Global rate limit per IP (per minute), and a stricter `api` policy
- In-process concurrency cap (default 10)
- Free-tier guard may block most paths (503) depending on config
- Security headers enforce CSP; avoid inline scripts unless nonce is injected

## Behavior Notes

- Localization: content depends on `culture` (`en`/`ru`) from query or cookie
- Cart is session-backed; no cross-session persistence
- No authentication is required; avoid posting sensitive data

## Stability

- Public routes and JSON schema are stable for demos; breaking changes will be reflected in `docs/` and release notes

