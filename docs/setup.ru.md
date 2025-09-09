# Установка

## Требования

- .NET SDK 8.0+

## Локальный запуск

- Из корня репозитория: `dotnet run --project SocksShoppingStore`
- Откройте выведенный HTTPS‑адрес (например, `https://localhost:5001`)
- Health‑проверка: `GET /healthz`

## Конфигурация

Правьте `SocksShoppingStore/appsettings.json`:

- `FreeTier`: включение защиты и allowlist путей
- `RateLimiting`: `GlobalPerMinute`, `ApiPerMinute`
- `Concurrency`: `MaxConcurrentRequests`
- `Status`: `AllowIPs` для `/_status`, `LatencyWindowSize`
- `Legal`: имя контроллера и контактный email

Для dev‑переопределений используйте `appsettings.Development.json`.

## Локализация

- Культура по умолчанию: `en` (`en-US` для чисел)
- Поддерживаются UI культуры: `en`, `ru`
- Переключение через POST cookie (`/Localization/Set`) или query `?culture=en|ru`

## Замечание про SSL

Для HTTPS нужен dev‑сертификат ASP.NET. Если отсутствует, установите:

- `dotnet dev-certs https --trust`

