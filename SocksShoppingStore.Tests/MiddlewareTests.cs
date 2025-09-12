using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Allure.NUnit;
using SocksShoppingStore.Middleware;
using System.Threading.Tasks;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("Unit")]
    public class MiddlewareTests
    {
        [Test]
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
        public async Task FreeTier_BlockAll_Returns503_ExceptAllowlisted()
        {
            var services = new ServiceCollection();
            services.Configure<FreeTierOptions>(o => { o.Enabled = true; o.BlockAllTraffic = true; o.AllowPaths = new[] { "/healthz" }; });
            var sp = services.BuildServiceProvider();

            var ctx = new DefaultHttpContext();
            ctx.Request.Path = "/";
            ctx.RequestServices = sp;
            var mw = new FreeTierGuardMiddleware(_ => Task.CompletedTask, sp.GetRequiredService<IOptions<FreeTierOptions>>());
            await mw.InvokeAsync(ctx);
            Assert.That(ctx.Response.StatusCode, Is.EqualTo(StatusCodes.Status503ServiceUnavailable));

            var ctx2 = new DefaultHttpContext();
            ctx2.Request.Path = "/healthz";
            ctx2.RequestServices = sp;
            var nextHit = false;
            var mw2 = new FreeTierGuardMiddleware(_ => { nextHit = true; return Task.CompletedTask; }, sp.GetRequiredService<IOptions<FreeTierOptions>>());
            await mw2.InvokeAsync(ctx2);
            Assert.That(nextHit, Is.True);
        }

        [Test]
        public async Task ConcurrencyLimiter_Blocks_When_Saturated()
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

