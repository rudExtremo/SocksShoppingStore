# Setup

## Requirements

- .NET SDK 8.0+

## Local Run

- From repo root: `dotnet run --project SocksShoppingStore`
- Open the printed HTTPS URL (e.g., `https://localhost:5001`)
- Health probe: `GET /healthz`

## Configuration

Edit `SocksShoppingStore/appsettings.json`:

- `FreeTier`: enable guard and allowlist paths
- `RateLimiting`: `GlobalPerMinute`, `ApiPerMinute`
- `Concurrency`: `MaxConcurrentRequests`
- `Status`: `AllowIPs` for `/_status`, `LatencyWindowSize`
- `Legal`: controller name and contact email

For development overrides, use `appsettings.Development.json`.

## Localization

- Default culture: `en` (`en-US` for numbers)
- Supported UI cultures: `en`, `ru`
- Switch via cookie POST (`/Localization/Set`) or query `?culture=en|ru`

## SSL Note

ASP.NET dev cert is required for HTTPS. If missing, install with:

- `dotnet dev-certs https --trust`

