# Метрики

Минимальные метрики запросов собираются в процессе и отдаются по `GET /_status`.

## Эндпоинт

- Путь: `/_status`
- Ограничение: allowlist IP (`Status.AllowIPs` в `appsettings.json`)
- Тело: JSON со счётчиками и перцентилями латенции (p50/p95/p99)

Пример:

```json
{
  "counts": { "ok": 1234, "too_many": 12, "unavailable": 0 },
  "latency_ms": { "p50": 3.1, "p95": 10.2, "p99": 25.7, "samples": 200 }
}
```

## Настройки

- `Status.LatencyWindowSize`: сколько последних замеров хранить (по умолчанию 200)
- `Status.AllowIPs`: список IP, которым разрешён доступ к метрикам

