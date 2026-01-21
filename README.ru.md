# SocksShoppingStore

Небольшое демо-приложение ASP.NET Core MVC для просмотра «профессия‑тематических» носков, добавления в корзину (сессия), и демонстрации базовых производственных аспектов: security headers, rate limiting, ограничение конкуренции, метрики, локализация.

- Технологии: ASP.NET Core 8 MVC, Razor Views, in-memory репозиторий, session cart, локализация EN/RU.
- Безопасность: CSP и защищённые заголовки, free-tier guard, глобальные и API‑лимиты, простой лимитер конкуренции.
- Эксплуатация: health endpoint, лёгкие метрики запросов, конфигурация через `appsettings.json`.

## Быстрый старт

- Требуется: .NET SDK 8.0+
- Запуск: `dotnet run --project SocksShoppingStore`
- Открыть: `https://localhost:5001` (или URL из консоли)
- Health: `GET /healthz` → `OK`

## Карта репозитория

- `SocksShoppingStore/Controllers` — MVC контроллеры (`Home`, `Products`, `ProductsApi`, `Cart`, `Checkout`, `StripeWebhook`, `Legal`, `Localization`).
- `SocksShoppingStore/Middleware` — `SecurityHeaders`, `FreeTierGuard`, `ConcurrencyLimiter`.
- `SocksShoppingStore/Services` — `ProductCatalogLocalizer`, `RateOptions`, `RequestMetrics`.
- `SocksShoppingStore/Data` — in-memory `ProductRepository`.
- `SocksShoppingStore/Models` — `Sock`, `ShoppingCart`, `CartItem`, `ErrorViewModel`.
- `SocksShoppingStore/Views` — Razor Views (UI), `wwwroot` — статика.
- `SocksShoppingStore/Program.cs` — пайплайн, DI, локализация, сессии, лимиты, метрики.

## Документация

- Обзор (EN): `docs/overview.md`
- Архитектура (EN): `docs/architecture.md`
- Установка (EN): `docs/setup.md`
- Тестирование (EN): `docs/testing.md`
- Гайд для агента: `docs/SOCKS_AGENT_GUIDE.md`

## Лицензия

См. `LICENSE`.

