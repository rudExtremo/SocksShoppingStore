using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text;

namespace SocksShoppingStore.Middleware
{
    public class FreeTierOptions
    {
        public bool Enabled { get; set; } = false;
        public bool BlockAllTraffic { get; set; } = false;
        public bool BlockOnAzure { get; set; } = false;
        public string[] AllowPaths { get; set; } = new[] { "/healthz", "/_status" };
    }

    public class FreeTierGuardMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly FreeTierOptions _options;

        public FreeTierGuardMiddleware(RequestDelegate next, IOptions<FreeTierOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_options.Enabled)
            {
                var path = context.Request.Path.HasValue ? context.Request.Path.Value! : string.Empty;
                var allowed = _options.AllowPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));

                bool runningOnAzure = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"))
                                   || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AZURE_HTTP_USER_AGENT"));

                if ((_options.BlockAllTraffic || (_options.BlockOnAzure && runningOnAzure)) && !allowed)
                {
                    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    context.Response.ContentType = "text/html; charset=utf-8";
                    var html = @"<!doctype html><html lang=\"ru\"><head><meta charset=\"utf-8\"><title>Временно недоступно</title>
                                <meta name=\"robots\" content=\"noindex\"></head><body>
                                <h1>Сервис временно недоступен</h1>
                                <p>Проект запущен в режиме Free Tier. Чтобы избежать расходов, доступ временно ограничён.</p>
                                <p>Проверьте настройки FreeTier или переменные окружения.</p>
                                </body></html>";
                    await context.Response.WriteAsync(html, Encoding.UTF8);
                    return;
                }
            }

            await _next(context);
        }
    }
}

