# Security and Limits

## Security Headers

Applied by `SecurityHeadersMiddleware`:

- `Content-Security-Policy` with per-request nonce
- `X-Frame-Options: DENY`
- `X-Content-Type-Options: nosniff`
- `Referrer-Policy: no-referrer`
- `Permissions-Policy` denying sensitive features

## Free-Tier Guard

Configured via `FreeTier` in `appsettings.json`:

- `Enabled`: turn guard on/off
- `BlockAllTraffic`: return 503 except allowlisted paths
- `BlockOnAzure`: auto-block when running on Azure (env detection)
- `AllowPaths`: paths that remain available (e.g., `/healthz`, `/_status`)

## Rate Limiting

- Global fixed-window limiter per IP (per minute)
- Named `api` policy for `/api/products` per IP
- Rejected requests return 429 with HTML/JSON depending on `Accept`

## Concurrency Limiting

- In-process cap using `SemaphoreSlim` (default `10`)
- On saturation, returns `429 Too Many Requests`

