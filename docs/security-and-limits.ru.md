# Безопасность и лимиты

## Security headers

Применяются в `SecurityHeadersMiddleware`:

- `Content-Security-Policy` с nonce на запрос
- `X-Frame-Options: DENY`
- `X-Content-Type-Options: nosniff`
- `Referrer-Policy: no-referrer`
- `Permissions-Policy` с запретом чувствительных возможностей

## Free‑Tier Guard

Настраивается через `FreeTier` в `appsettings.json`:

- `Enabled`: включение/выключение защиты
- `BlockAllTraffic`: отдавать 503, кроме allowlisted путей
- `BlockOnAzure`: авто‑блокировка при запуске на Azure (по env)
- `AllowPaths`: остающиеся доступными пути (например, `/healthz`, `/_status`)

## Ограничение скорости

- Глобальный лимитер фиксированного окна на IP (в минуту)
- Именованная политика `api` для `/api/products` на IP
- При отказе возвращается 429 (HTML/JSON в зависимости от `Accept`)

## Ограничение конкуренции

- In‑process лимит через `SemaphoreSlim` (по умолчанию `10`)
- При насыщении возвращается `429 Too Many Requests`

