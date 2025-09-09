# Metrics

Minimal request metrics are collected in-process and exposed via `GET /_status`.

## Endpoint

- Path: `/_status`
- Restriction: IP allowlist (`Status.AllowIPs` in `appsettings.json`)
- Body: JSON with counts and latency percentiles (p50/p95/p99)

Example:

```json
{
  "counts": { "ok": 1234, "too_many": 12, "unavailable": 0 },
  "latency_ms": { "p50": 3.1, "p95": 10.2, "p99": 25.7, "samples": 200 }
}
```

## Configuration

- `Status.LatencyWindowSize`: number of latest samples to keep (default 200)
- `Status.AllowIPs`: list of IPs allowed to read metrics

