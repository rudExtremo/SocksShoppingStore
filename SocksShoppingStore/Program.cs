using System.Globalization;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using SocksShoppingStore.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Время жизни сессии
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var cultureInfo = new CultureInfo("fr-FR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Bind FreeTier options from configuration / env vars
builder.Services.Configure<FreeTierOptions>(builder.Configuration.GetSection("FreeTier"));

// Rate limiting policies (safe defaults for free tier)
builder.Services.AddRateLimiter(options =>
{
    // Global: 40 req/min per IP
    options.GlobalLimiter = httpContext =>
        RateLimitPartition.GetIpAddressLimiter(httpContext, ip => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 40, // 40 req/min per IP
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });

    // API: 20 req/min per IP
    options.AddPolicy("api", httpContext =>
        RateLimitPartition.GetIpAddressLimiter(httpContext, ip => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 20, // stricter for API
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        }));

    // Friendly 429 response
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        var accept = context.HttpContext.Request.Headers["Accept"].ToString();
        if (!string.IsNullOrEmpty(accept) && accept.Contains("application/json", StringComparison.OrdinalIgnoreCase))
        {
            context.HttpContext.Response.ContentType = "application/json";
            await context.HttpContext.Response.WriteAsync("{\"error\":\"Too Many Requests\",\"message\":\"Rate limit exceeded. Please retry later.\"}", token);
        }
        else
        {
            context.HttpContext.Response.ContentType = "text/html; charset=utf-8";
            await context.HttpContext.Response.WriteAsync("<h1>Too Many Requests</h1><p>Rate limit exceeded. Please retry later.</p>", token);
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Free-tier guard to avoid accidental costs (can block on Azure)
app.UseMiddleware<FreeTierGuardMiddleware>();

// Global concurrency limiter (fast-fail over 10 concurrent requests)
app.UseMiddleware<ConcurrencyLimiterMiddleware>();

// Apply default rate limiter
app.UseRateLimiter();

app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Lightweight health endpoint (allowed in FreeTier mode)
app.MapGet("/healthz", () => Results.Ok("OK"));

app.Run();

