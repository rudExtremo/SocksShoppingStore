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

- `SocksShoppingStore/Controllers` — MVC контроллеры (`Home`, `Products`, `ProductsApi`, `Cart`, `Legal`, `Localization`).
- `SocksShoppingStore/Middleware` — `SecurityHeaders`, `FreeTierGuard`, `ConcurrencyLimiter`.
- `SocksShoppingStore/Services` — `ProductCatalogLocalizer`, `RateOptions`, `RequestMetrics`.
- `SocksShoppingStore/Data` — in-memory `ProductRepository`.
- `SocksShoppingStore/Models` — `Sock`, `ShoppingCart`, `CartItem`, `ErrorViewModel`.
- `SocksShoppingStore/Views` — Razor Views (UI), `wwwroot` — статика.
- `SocksShoppingStore/Program.cs` — пайплайн, DI, локализация, сессии, лимиты, метрики.
- `.codex` — исторические RTF/PDF (содержимое перенесено в `docs/`).

## Документация

- Обзор: `docs/overview.ru.md`
- Архитектура: `docs/architecture.ru.md`
- Установка: `docs/setup.ru.md`
- API: `docs/api/products.ru.md`
- Локализация: `docs/internationalization.ru.md`
- Безопасность и лимиты: `docs/security-and-limits.ru.md`
- Метрики: `docs/metrics.ru.md`
- Дорожная карта: `docs/roadmap.ru.md`
- Вклад: `docs/contributing.ru.md`

Англоязычные версии находятся рядом без суффикса `.ru`.

## Лицензия

См. `LICENSE`.

