using System.Collections.Concurrent;

namespace SocksShoppingStore
{
    public class RequestMetrics
    {
        private long _ok;
        private long _tooMany;
        private long _unavailable;
        private readonly ConcurrentQueue<double> _latencies = new();
        private readonly int _windowSize;

        public RequestMetrics(int windowSize = 200)
        {
            _windowSize = windowSize > 0 ? windowSize : 200;
        }

        public void Record(int statusCode, double ms)
        {
            if (statusCode >= 200 && statusCode < 300) Interlocked.Increment(ref _ok);
            else if (statusCode == 429) Interlocked.Increment(ref _tooMany);
            else if (statusCode == 503) Interlocked.Increment(ref _unavailable);

            _latencies.Enqueue(ms);
            while (_latencies.Count > _windowSize && _latencies.TryDequeue(out _)) { }
        }

        public object Snapshot()
        {
            var arr = _latencies.ToArray();
            Array.Sort(arr);
            double p(double q)
            {
                if (arr.Length == 0) return 0;
                var idx = (int)Math.Clamp(Math.Round(q * (arr.Length - 1)), 0, arr.Length - 1);
                return arr[idx];
            }
            return new
            {
                counts = new { ok = _ok, too_many = _tooMany, unavailable = _unavailable },
                latency_ms = new { p50 = p(0.50), p95 = p(0.95), p99 = p(0.99), samples = arr.Length }
            };
        }
    }
}

