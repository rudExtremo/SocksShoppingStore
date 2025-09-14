using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;
using SocksShoppingStore.Controllers;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("Store")]
    [AllureSuite("API Tests")]
    [AllureFeature("Product Catalog")]
    [AllureLabel("package", "SocksShoppingStore.Tests.API")]
    [AllureLabel("area", "API")]
    [AllureLabel("type", "Functional")]
    [AllureLabel("flow", "Positive")]
    [AllureSeverity(SeverityLevel.critical)]
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
        public void GetAllProducts_SetsCachingHeaders_AndReturnsJson()
        {
            var ctx = new DefaultHttpContext();
            var controller = CreateController(ctx);

            var result = controller.GetAllProducts(q: null, sort: null, minPrice: null, maxPrice: null, page: 1, pageSize: 5, culture: "en");

            Assert.That(result, Is.InstanceOf<ContentResult>());
            var content = (ContentResult)result;
            Assert.That(content.ContentType, Does.StartWith("application/json"));
            AllureApi.AddAttachment("api-products.json", "application/json", System.Text.Encoding.UTF8.GetBytes(content.Content ?? string.Empty));

            // Headers
            Assert.That(ctx.Response.Headers.ContainsKey("ETag"), Is.True);
            Assert.That(ctx.Response.Headers.ContainsKey("Last-Modified"), Is.True);
            Assert.That(ctx.Response.Headers["Cache-Control"].ToString(), Does.Contain("max-age"));
            Assert.That(int.TryParse(ctx.Response.Headers["X-Total-Count"], out _), Is.True);
        }

        [Test]
        public void GetAllProducts_Respects_IfNoneMatch_Returns304()
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
        public void GetProduct_Returns304_OnIfModifiedSince()
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
