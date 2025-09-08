using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SocksShoppingStore.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        public SecurityHeadersMiddleware(RequestDelegate next) { _next = next; }

        public async Task InvokeAsync(HttpContext context)
        {
            // Per-request nonce for potential inline scripts (not used by default)
            var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            context.Items["CspNonce"] = nonce;
            var headers = context.Response.Headers;
            headers["X-Frame-Options"] = "DENY";
            headers["X-Content-Type-Options"] = "nosniff";
            headers["Referrer-Policy"] = "no-referrer";
            headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
            // Allow own content; cdnjs for CSS/Fonts; inline styles for Bootstrap helpers
            headers["Content-Security-Policy"] =
                $"default-src 'self'; script-src 'self' 'nonce-{nonce}'; style-src 'self' https://cdnjs.cloudflare.com 'unsafe-inline'; " +
                "img-src 'self' data:; font-src 'self' https://cdnjs.cloudflare.com; connect-src 'self'";

            await _next(context);
        }
    }
}
