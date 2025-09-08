using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

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
                    var html = "<!doctype html><html lang=\"en\"><head><meta charset=\"utf-8\"><title>Temporarily unavailable</title><meta name=\"robots\" content=\"noindex\"></head><body><h1>Service temporarily unavailable</h1><p>Running in Free Tier mode. Access is restricted to prevent costs.</p><p>Check FreeTier settings or environment variables.</p></body></html>";
                    await context.Response.WriteAsync(html);
                    return;
                }
            }

            await _next(context);
        }
    }
}

