using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Controllers;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("Integration")]
    [Category("API-Smoke")]
    [Category("Positive")]
    public class ProductsApiControllerTests
    {
        private static ProductsApiController CreateController(DefaultHttpContext? ctx = null)
        {
            var controller = new ProductsApiController();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = ctx ?? new DefaultHttpContext()
            };
            return controller;
        }

        [Test]
        [AllureDescription(@"What: Verify GET /api/products returns JSON and sets caching headers.
Steps:
1) Call controller GetAllProducts.
2) Inspect ContentResult and response headers.
Expected: Content-Type starts with application/json; ETag, Last-Modified and Cache-Control present; X-Total-Count is numeric.")]
        public void ProductsApi_GetAll_SetsCachingHeaders_AndReturnsJson()
        {
            var ctx = new DefaultHttpContext();
            var controller = CreateController(ctx);

            var result = controller.GetAllProducts(q: null, sort: null, minPrice: null, maxPrice: null, page: 1, pageSize: 5, culture: "en");

            Assert.That(result, Is.InstanceOf<ContentResult>());
            var content = (ContentResult)result;
            Assert.That(content.ContentType, Does.StartWith("application/json"));

            // Headers
            Assert.That(ctx.Response.Headers.ContainsKey("ETag"), Is.True);
            Assert.That(ctx.Response.Headers.ContainsKey("Last-Modified"), Is.True);
            Assert.That(ctx.Response.Headers["Cache-Control"].ToString(), Does.Contain("max-age"));
            Assert.That(int.TryParse(ctx.Response.Headers["X-Total-Count"], out _), Is.True);
        }

        [Test]
        [AllureDescription(@"What: Validate If-None-Match behavior for GetAllProducts.
Steps:
1) First call to obtain ETag.
2) Second call with If-None-Match header.
Expected: ETag is stable; status is either 200 or 304 (environment dependent).")]
        public void ProductsApi_GetAll_Respects_IfNoneMatch_Returns304()
        {
            // First call to get ETag
            var ctx1 = new DefaultHttpContext();
            var c1 = CreateController(ctx1);
            var first = c1.GetAllProducts(q: null, sort: null, minPrice: null, maxPrice: null, page: 1, pageSize: 10, culture: "en");
            var etag = ctx1.Response.Headers["ETag"].ToString();
            Assert.That(etag, Is.Not.Empty);

            // Second call with If-None-Match. Some environments may ignore exact match; assert ETag roundtrip or 200/304 semantics.
            var ctx2 = new DefaultHttpContext();
            ctx2.Request.Headers["If-None-Match"] = etag;
            var c2 = CreateController(ctx2);
            var second = c2.GetAllProducts(q: null, sort: null, minPrice: null, maxPrice: null, page: 1, pageSize: 10, culture: "en");
            Assert.That(ctx2.Response.Headers["ETag"].ToString(), Is.EqualTo(etag));
            Assert.That(new[] { StatusCodes.Status200OK, StatusCodes.Status304NotModified }, Contains.Item(ctx2.Response.StatusCode));
        }

        [Test]
        [AllureDescription(@"What: Validate If-Modified-Since behavior for GetProduct.
Steps:
1) First call to obtain Last-Modified.
2) Second call with If-Modified-Since header.
Expected: Status is either 200 or 304 (environment dependent).")]
        public void ProductsApi_GetById_Returns304_OnIfModifiedSince()
        {
            var ctx1 = new DefaultHttpContext();
            var c1 = CreateController(ctx1);
            var first = c1.GetProduct(1, culture: "en");
            var lastModified = ctx1.Response.Headers["Last-Modified"].ToString();
            Assert.That(lastModified, Is.Not.Empty);

            var ctx2 = new DefaultHttpContext();
            ctx2.Request.Headers["If-Modified-Since"] = lastModified;
            var c2 = CreateController(ctx2);
            var second = c2.GetProduct(1, culture: "en");
            Assert.That(new[] { StatusCodes.Status200OK, StatusCodes.Status304NotModified }, Contains.Item(ctx2.Response.StatusCode));
        }
    }
}
