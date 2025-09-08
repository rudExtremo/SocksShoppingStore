using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace SocksShoppingStore.Middleware
{
    public class ConcurrencyLimiterMiddleware
    {
        // In-process cap for concurrent requests
        private readonly SemaphoreSlim _semaphore;
        private readonly RequestDelegate _next;
        private readonly int _max;

        public ConcurrencyLimiterMiddleware(RequestDelegate next, IOptions<ConcurrencyOptions> options)
        {
            _next = next;
            _max = options.Value.MaxConcurrentRequests > 0 ? options.Value.MaxConcurrentRequests : 10;
            _semaphore = new SemaphoreSlim(_max, _max);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Try to avoid building up long queues; fail fast if saturated
            if (!await _semaphore.WaitAsync(0))
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Too Many Requests - server is busy");
                return;
            }

            try
            {
                await _next(context);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    public class ConcurrencyOptions
    {
        public int MaxConcurrentRequests { get; set; } = 10;
    }
}
