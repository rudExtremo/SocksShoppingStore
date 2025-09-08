using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SocksShoppingStore.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        public SecurityHeadersMiddleware(RequestDelegate next) { _next = next; }

        public async Task InvokeAsync(HttpContext context)
        {
            var headers = context.Response.Headers;
            headers["X-Frame-Options"] = "DENY";
            headers["X-Content-Type-Options"] = "nosniff";
            headers["Referrer-Policy"] = "no-referrer";
            headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
            // Allow own content; cdnjs for CSS/Fonts; inline styles for Bootstrap helpers
            headers["Content-Security-Policy"] =
                "default-src 'self'; script-src 'self'; style-src 'self' https://cdnjs.cloudflare.com 'unsafe-inline'; " +
                "img-src 'self' data:; font-src 'self' https://cdnjs.cloudflare.com; connect-src 'self'";

            await _next(context);
        }
    }
}

