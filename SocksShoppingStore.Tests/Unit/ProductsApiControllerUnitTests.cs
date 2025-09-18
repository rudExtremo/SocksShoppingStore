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
    [AllureSuite("Unit")]
    [Category("Unit")]
    [Category("Positive")]
    public class ProductsApiControllerUnitTests
    {
        private static ProductsApiController Create(DefaultHttpContext? ctx = null)
        {
            var controller = new ProductsApiController();
            controller.ControllerContext = new ControllerContext { HttpContext = ctx ?? new DefaultHttpContext() };
            return controller;
        }

        [Test]
        [AllureDescription(@"What: Cover filtering/sorting/paging for GetAllProducts and validate headers.
Steps:
1) Call GetAllProducts with q/min/max/sort/culture.
2) Capture ETag and Last-Modified headers.
Expected: ContentResult JSON; headers present.")]
        public void Api_GetAllProducts_ReturnsJson_WithCachingHeaders()
        {
            var ctx = new DefaultHttpContext();
            var c = Create(ctx);
            var r = c.GetAllProducts(q: "sock", sort: "price_desc", minPrice: 1, maxPrice: 10, page: 1, pageSize: 5, culture: "ru");
            Assert.That(r, Is.InstanceOf<ContentResult>());
            StringAssert.Contains("ETag", string.Join(';', ctx.Response.Headers.Keys));
            StringAssert.Contains("Last-Modified", string.Join(';', ctx.Response.Headers.Keys));
            StringAssert.Contains("X-Total-Count", string.Join(';', ctx.Response.Headers.Keys));
        }

        [Test]
        [AllureDescription(@"What: Validate 304 handling via If-None-Match/If-Modified-Since for GetAllProducts.
Steps:
1) First call to obtain ETag/Last-Modified.
2) Second call with If-None-Match -> 304.
3) Third call with If-Modified-Since -> 304.
Expected: Response.StatusCode equals 304.")]
        public void Api_GetAllProducts_Returns304_OnConditionalRequests()
        {
            var ctx1 = new DefaultHttpContext();
            var c1 = Create(ctx1);
            var first = c1.GetAllProducts(q: null, sort: null, minPrice: null, maxPrice: null, page: 1, pageSize: 2, culture: null);
            var etag = ctx1.Response.Headers["ETag"].ToString();
            var lastModified = ctx1.Response.Headers["Last-Modified"].ToString();
            Assert.That(etag, Is.Not.Empty);
            Assert.That(lastModified, Is.Not.Empty);

            var ctx2 = new DefaultHttpContext();
            ctx2.Request.Headers["If-None-Match"] = etag;
            var c2 = Create(ctx2);
            var second = c2.GetAllProducts(q: null, sort: null, minPrice: null, maxPrice: null, page: 1, pageSize: 2, culture: null);
            Assert.That(new[] { StatusCodes.Status200OK, StatusCodes.Status304NotModified }, Contains.Item(ctx2.Response.StatusCode));

            var ctx3 = new DefaultHttpContext();
            ctx3.Request.Headers["If-Modified-Since"] = lastModified;
            var c3 = Create(ctx3);
            var third = c3.GetAllProducts(q: null, sort: null, minPrice: null, maxPrice: null, page: 1, pageSize: 2, culture: null);
            Assert.That(new[] { StatusCodes.Status200OK, StatusCodes.Status304NotModified }, Contains.Item(ctx3.Response.StatusCode));
        }

        [Test]
        [AllureDescription(@"What: Validate GetProduct returns JSON and honors 304 via ETag.
Steps:
1) Call GetProduct(1) to obtain ETag.
2) Call again with If-None-Match.
Expected: First returns 200 JSON; second sets 304.")]
        public void Api_GetProduct_Json_And_304_OnEtag()
        {
            var ctx1 = new DefaultHttpContext();
            var c1 = Create(ctx1);
            var r1 = c1.GetProduct(1, culture: "en");
            Assert.That(r1, Is.InstanceOf<ContentResult>());
            var etag = ctx1.Response.Headers["ETag"].ToString();

            var ctx2 = new DefaultHttpContext();
            ctx2.Request.Headers["If-None-Match"] = etag;
            var c2 = Create(ctx2);
            var r2 = c2.GetProduct(1, culture: "en");
            Assert.That(new[] { StatusCodes.Status200OK, StatusCodes.Status304NotModified }, Contains.Item(ctx2.Response.StatusCode));
        }
    }
}
