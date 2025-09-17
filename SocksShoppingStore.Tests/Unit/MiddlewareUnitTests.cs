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
    [Category("Unit")]
    [Category("Security")]
    public class MiddlewareUnitTests
    {
        [Test]
        [AllureDescription(@"What: Ensure SecurityHeadersMiddleware adds core security headers and nonce.
Steps:
1) Invoke middleware with a fresh HttpContext.
Expected: CSP header present; nonce stored; X-Frame-Options=DENY.")]
        public async Task Middleware_SecurityHeaders_AddsCoreHeaders()
        {
            var ctx = new DefaultHttpContext();
            var mw = new SecurityHeadersMiddleware(_ => Task.CompletedTask);
            await mw.InvokeAsync(ctx);
            Assert.That(ctx.Response.Headers.ContainsKey("Content-Security-Policy"), Is.True);
            Assert.That(ctx.Items.ContainsKey("CspNonce"), Is.True);
            Assert.That(ctx.Response.Headers["X-Frame-Options"].ToString(), Is.EqualTo("DENY"));
        }

        [Test]
        [AllureDescription(@"What: Verify FreeTierGuardMiddleware blocks all except allowlisted paths.
Steps:
1) Configure options: Enabled=true, BlockAllTraffic=true, AllowPaths=['/healthz'].
2) '/' -> 503; '/healthz' -> next().")]
        public async Task Middleware_FreeTier_BlockAll_Returns503_ExceptAllowlisted()
        {
            var services = new ServiceCollection();
            services.Configure<FreeTierOptions>(o => { o.Enabled = true; o.BlockAllTraffic = true; o.AllowPaths = new[] { "/healthz" }; });
            var sp = services.BuildServiceProvider();

            var ctx = new DefaultHttpContext { RequestServices = sp };
            var mw = new FreeTierGuardMiddleware(_ => Task.CompletedTask, sp.GetRequiredService<IOptions<FreeTierOptions>>());
            await mw.InvokeAsync(ctx);
            Assert.That(ctx.Response.StatusCode, Is.EqualTo(StatusCodes.Status503ServiceUnavailable));

            var ctx2 = new DefaultHttpContext { RequestServices = sp };
            ctx2.Request.Path = "/healthz";
            var passed = false;
            var mw2 = new FreeTierGuardMiddleware(_ => { passed = true; return Task.CompletedTask; }, sp.GetRequiredService<IOptions<FreeTierOptions>>());
            await mw2.InvokeAsync(ctx2);
            Assert.That(passed, Is.True);
        }

        [Test]
        [AllureDescription(@"What: Ensure ConcurrencyLimiterMiddleware returns 429 when saturated.
Steps:
1) Set MaxConcurrentRequests=1; hold first request; second returns 429.")]
        public async Task Middleware_ConcurrencyLimiter_Returns429_WhenSaturated()
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

