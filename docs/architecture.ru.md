# Архитектура

Классическое ASP.NET Core MVC‑приложение, разложенное по зонам ответственности:

- Controllers (`SocksShoppingStore/Controllers`): HTTP‑эндпоинты и координация UI
- Middleware (`SocksShoppingStore/Middleware`): сквозные аспекты пайплайна
- Services (`SocksShoppingStore/Services`): помощники локализации и опции
- Data (`SocksShoppingStore/Data`): источник данных в памяти
- Models (`SocksShoppingStore/Models`): доменные и view‑модели
- Views (`SocksShoppingStore/Views`): Razor UI; `wwwroot` — статические ресурсы

## Пайплайн запросов (Program.cs)

- HTTPS‑редирект и статика с кэш‑заголовками (7 дней)
- Локализация запросов (EN/RU) через провайдеры query и cookie
- Free‑tier guard: защита от случайных расходов/экспозиции деплоя
- Security headers (CSP, запрет фреймов, nosniff, referrer, permissions)
- Политика cookie (SameSite=Lax, HttpOnly, Secure по окружению)
- Ограничитель конкуренции (in‑process, быстрый отказ при насыщении)
- Ограничитель скорости (глобальный + именованный `api`) с дружелюбным телом 429
- Сессии и MVC‑роутинг
- Опер. эндпоинты: `GET /healthz`, `GET /robots.txt`, `GET /_status`

## Контроллеры

- `HomeController`: список каталога с поиском `q`; локализованные тексты
- `ProductsController`: детали товара (`/Products/Details?id=`)
- `ProductsApiController` (`[Route("api/products")]`): JSON‑выдача всех товаров; `[EnableRateLimiting("api")]`
- `CartController`: корзина в сессии (add/remove/delete/clear)
- `LegalController`: статичные страницы (TOS/Privacy)
- `LocalizationController`: POST‑переключение культуры с безопасным возвратом

## UI

- Поиск в шапке: компактный dropdown рядом с переключателем языка
- Фильтры каталога: Цена (min–max), Сортировка (кнопки с иконками)
- Ленивая загрузка: кнопка «Load more» запрашивает `/api/products?page=N&pageSize=M` и добавляет карточки на клиенте (без бесконечной прокрутки)

## Middleware

- `SecurityHeadersMiddleware`: CSP с nonce на запрос, запрет фреймов, `nosniff`, строгие referrer/permissions
- `FreeTierGuardMiddleware`: при включении и не‑allowlisted путях возвращает 503 (блокировка на Azure или целиком по настройке)
- `ConcurrencyLimiterMiddleware`: ограничение конкурентных запросов через `SemaphoreSlim`, при насыщении — 429

## Services и Data

- `ProductCatalogLocalizer`: подстановка имени/описания товара по культуре (EN по умолчанию, RU карта)
- `IProductRepository`: абстракция хранилища товаров
- `LegacyProductRepository`: обёртка над in‑memory списком (по умолчанию)
- `JsonProductRepository`: опциональное хранение в JSON (настраиваемый путь)
- `RequestMetrics`: скользящее окно латенсий + счётчики; доступно через `/_status`

## Конфигурация

`SocksShoppingStore/appsettings.json`:

- `FreeTier`: переключатель, детекция Azure, allowlist путей
- `RateLimiting`: `GlobalPerMinute`, `ApiPerMinute`
- `Concurrency`: `MaxConcurrentRequests`
- `Status`: `AllowIPs`, `LatencyWindowSize`
- `Legal`: имя контроллера и контактный email
- `Repository`: провайдер `InMemory|Json` и путь к JSON
