using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using SocksShoppingStore.Middleware;
using Microsoft.AspNetCore.StaticFiles;
using System.Text.Json;
using SocksShoppingStore.Controllers;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using SocksShoppingStore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources"); // UI localization scaffolding
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Время жизни сессии
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Default culture for numbers (we will format prices explicitly as EUR)
var defaultCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

// Bind options from configuration
builder.Services.Configure<FreeTierOptions>(builder.Configuration.GetSection("FreeTier"));
builder.Services.Configure<ConcurrencyOptions>(builder.Configuration.GetSection("Concurrency"));
builder.Services.Configure<LegalOptions>(builder.Configuration.GetSection("Legal"));

var rateOptions = builder.Configuration.GetSection("RateLimiting").Get<RateOptions>() ?? new RateOptions();

// Rate limiting policies (safe defaults for free tier)
builder.Services.AddRateLimiter(options =>
{
    // Global per-IP window limiter
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: key => new FixedWindowRateLimiterOptions
            {
                PermitLimit = rateOptions.GlobalPerMinute,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    // API per-IP window limiter
    options.AddPolicy("api", httpContext =>
        PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: key => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = rateOptions.ApiPerMinute,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                })));

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
// Static files with cache headers
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Cache static assets for 7 days to reduce traffic
        const int days = 7;
        ctx.Context.Response.Headers["Cache-Control"] = $"public, max-age={days * 24 * 3600}";
    }
});

app.UseRouting();

// Request localization: support EN/RU via cookie and query string
var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("ru") };
var locOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
locOptions.AddInitialRequestCultureProvider(new QueryStringRequestCultureProvider());
locOptions.AddInitialRequestCultureProvider(new CookieRequestCultureProvider());
app.UseRequestLocalization(locOptions);

// Free-tier guard to avoid accidental costs (can block on Azure)
app.UseMiddleware<FreeTierGuardMiddleware>();

// Security headers (CSP, frame, sniffing, referrer, permissions)
app.UseMiddleware<SecurityHeadersMiddleware>();

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

// Robots.txt to discourage crawling on free tier
app.MapGet("/robots.txt", () => Results.Text("User-agent: *\nDisallow: /\n", "text/plain"));

// Metrics storage and endpoint
var metrics = new RequestMetrics(
    builder.Configuration.GetSection("Status:LatencyWindowSize").Get<int?>() ?? 200);

app.Use(async (ctx, next) =>
{
    var sw = System.Diagnostics.Stopwatch.StartNew();
    try { await next(); }
    finally
    {
        sw.Stop();
        metrics.Record(ctx.Response.StatusCode, sw.Elapsed.TotalMilliseconds);
    }
});

app.MapGet("/_status", (HttpContext ctx) =>
{
    // Allowlist by IP
    var allow = builder.Configuration.GetSection("Status:AllowIPs").Get<string[]>() ?? new[] {"127.0.0.1","::1"};
    var remoteIp = ctx.Connection.RemoteIpAddress?.ToString() ?? "";
    if (!allow.Contains(remoteIp)) return Results.StatusCode(StatusCodes.Status403Forbidden);
    var snapshot = metrics.Snapshot();
    return Results.Json(snapshot, new JsonSerializerOptions { WriteIndented = true });
});

app.Run();

