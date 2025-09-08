using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SocksShoppingStore.Middleware
{
    public class ConcurrencyLimiterMiddleware
    {
        // Global in-process cap for concurrent requests
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(10, 10);
        private readonly RequestDelegate _next;

        public ConcurrencyLimiterMiddleware(RequestDelegate next)
        {
            _next = next;
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
}

