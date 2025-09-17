using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Middleware;
using System.Threading.Tasks;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("API-Smoke")]
    [Category("Security")]
    public class ApiDependenciesMiddlewareTests
    {
        [Test]
        [AllureDescription(@"What: Verify SecurityHeaders middleware behavior (API dependency).
Steps:
1) Invoke with new HttpContext.
Expected: CSP header and nonce present; X-Frame-Options=DENY.")]
        public async Task SecurityHeaders_AddsCoreHeaders()
        {
            var ctx = new DefaultHttpContext();
            var mw = new SecurityHeadersMiddleware(_ => Task.CompletedTask);
            await mw.InvokeAsync(ctx);
            Assert.That(ctx.Response.Headers.ContainsKey("Content-Security-Policy"), Is.True);
            Assert.That(ctx.Items.ContainsKey("CspNonce"), Is.True);
            Assert.That(ctx.Response.Headers["X-Frame-Options"].ToString(), Is.EqualTo("DENY"));
        }

        [Test]
        [AllureDescription(@"What: FreeTier guard blocks except allowlisted (API dependency).
Steps:
1) Enabled=true, BlockAll=true, AllowPaths=['/healthz'].
Expected: '/' -> 503; '/healthz' passes.")]
        public async Task FreeTier_BlockAll_ExceptAllowlisted()
        {
            var services = new ServiceCollection();
            services.Configure<FreeTierOptions>(o => { o.Enabled = true; o.BlockAllTraffic = true; o.AllowPaths = new[] { "/healthz" }; });
            var sp = services.BuildServiceProvider();

            var ctx = new DefaultHttpContext { RequestServices = sp };
            var mw = new FreeTierGuardMiddleware(_ => Task.CompletedTask, sp.GetRequiredService<IOptions<FreeTierOptions>>());
            await mw.InvokeAsync(ctx);
            Assert.That(ctx.Response.StatusCode, Is.EqualTo(StatusCodes.Status503ServiceUnavailable));

            var ctx2 = new DefaultHttpContext { RequestServices = sp, Request = { Path = "/healthz" } };
            var ok = false;
            var mw2 = new FreeTierGuardMiddleware(_ => { ok = true; return Task.CompletedTask; }, sp.GetRequiredService<IOptions<FreeTierOptions>>());
            await mw2.InvokeAsync(ctx2);
            Assert.That(ok, Is.True);
        }

        [Test]
        [AllureDescription(@"What: Concurrency limiter returns 429 when saturated (API dependency).
Steps:
1) MaxConcurrentRequests=1; hold first request; invoke second.
Expected: Second response status is 429.")]
        public async Task ConcurrencyLimiter_Returns429_WhenSaturated()
        {
            var services = new ServiceCollection();
            services.Configure<ConcurrencyOptions>(o => o.MaxConcurrentRequests = 1);
            var sp = services.BuildServiceProvider();

            var gate = new TaskCompletionSource();
            var mw = new ConcurrencyLimiterMiddleware(async _ => await gate.Task, sp.GetRequiredService<IOptions<ConcurrencyOptions>>());

            var ctx1 = new DefaultHttpContext();
            var t1 = mw.InvokeAsync(ctx1);

            var ctx2 = new DefaultHttpContext();
            await mw.InvokeAsync(ctx2);
            Assert.That(ctx2.Response.StatusCode, Is.EqualTo(StatusCodes.Status429TooManyRequests));

            gate.SetResult();
            await t1;
        }
    }
}
