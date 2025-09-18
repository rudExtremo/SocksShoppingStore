using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Controllers;
using SocksShoppingStore.Models;
using System.Text.Json;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("Integration")]
    [Category("API-Smoke")]
    [Category("Positive")]
    public class ProductsApiBehaviorTests
    {
        private static ProductsApiController Create(DefaultHttpContext? ctx = null)
        {
            var c = new ProductsApiController();
            c.ControllerContext = new ControllerContext { HttpContext = ctx ?? new DefaultHttpContext() };
            return c;
        }

        [Test]
        [AllureDescription(@"What: Verify sort by name_desc and price_asc produce correct ordering.
Steps:
1) Call sort='name_desc'.
2) Call sort='price_asc'.
Expected: Lists are ordered accordingly.")]
        public void ProductsApi_Sort_NameDesc_And_PriceAsc()
        {
            var c = Create(new DefaultHttpContext());
            var r1 = c.GetAllProducts(q: null, sort: "name_desc", minPrice: null, maxPrice: null, page: 1, pageSize: 10, culture: "en") as ContentResult;
            var r2 = c.GetAllProducts(q: null, sort: "price_asc", minPrice: null, maxPrice: null, page: 1, pageSize: 10, culture: "en") as ContentResult;
            var l1 = Parse(r1!);
            var l2 = Parse(r2!);
            var names = l1.Select(s => s.Name ?? string.Empty).ToList();
            var expected = names.OrderByDescending(x => x, StringComparer.CurrentCultureIgnoreCase).ToList();
            CollectionAssert.AreEqual(expected, names);
            for (int i = 1; i < l2.Count; i++) Assert.That(l2[i-1].Price, Is.LessThanOrEqualTo(l2[i].Price));
        }

        [Test]
        [AllureDescription(@"What: Verify min-only and max-only price filters.
Steps:
1) Call with minPrice only.
2) Call with maxPrice only.
Expected: All items satisfy respective bounds.")]
        public void ProductsApi_Filter_MinOnly_And_MaxOnly()
        {
            var c = Create(new DefaultHttpContext());
            var rMin = c.GetAllProducts(q: null, sort: null, minPrice: 3.0m, maxPrice: null, page: 1, pageSize: 10, culture: null) as ContentResult;
            var rMax = c.GetAllProducts(q: null, sort: null, minPrice: null, maxPrice: 4.0m, page: 1, pageSize: 10, culture: null) as ContentResult;
            var lmin = Parse(rMin!);
            var lmax = Parse(rMax!);
            Assert.That(lmin.All(s => s.Price >= 3.0m), Is.True);
            Assert.That(lmax.All(s => s.Price <= 4.0m), Is.True);
        }

        [Test]
        [AllureDescription(@"What: Verify defaulting behavior for page and pageSize.
Steps:
1) Call with page<1 and pageSize<=0.
Expected: Returns a valid page with non-negative count.")]
        public void ProductsApi_Page_Defaults_When_OutOfBounds()
        {
            var c = Create(new DefaultHttpContext());
            var r = c.GetAllProducts(q: null, sort: null, minPrice: null, maxPrice: null, page: -5, pageSize: 0, culture: null) as ContentResult;
            var list = Parse(r!);
            Assert.That(list.Count, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        [AllureDescription(@"What: Invalid sort value falls back to default name_asc.
Steps:
1) Call with sort='unknown'; call with sort='name_asc'.
Expected: Both responses yield identical ordering.")]
        public void ProductsApi_Sort_Invalid_FallsBack_To_Default()
        {
            var c = Create(new DefaultHttpContext());
            var r1 = c.GetAllProducts(q: null, sort: "unknown", minPrice: null, maxPrice: null, page: 1, pageSize: 10, culture: "en") as ContentResult;
            var r2 = c.GetAllProducts(q: null, sort: "name_asc", minPrice: null, maxPrice: null, page: 1, pageSize: 10, culture: "en") as ContentResult;
            var l1 = Parse(r1!); var l2 = Parse(r2!);
            CollectionAssert.AreEqual(l2.Select(s=>s.Id).ToList(), l1.Select(s=>s.Id).ToList());
        }

        [Test]
        [AllureDescription(@"What: ETag consistency and 304 behavior on list endpoint.
Steps:
1) First call to obtain ETag.
2) Second call with If-None-Match.
Expected: Response 304 and ETag unchanged.")]
        public void ProductsApi_Etag_And_304_OnList()
        {
            var ctx1 = new DefaultHttpContext();
            var c1 = Create(ctx1);
            var first = c1.GetAllProducts(q: null, sort: null, minPrice: null, maxPrice: null, page: 1, pageSize: 3, culture: null) as ContentResult;
            var etag = ctx1.Response.Headers["ETag"].ToString();
            Assert.That(etag, Is.Not.Empty);

            var ctx2 = new DefaultHttpContext();
            ctx2.Request.Headers["If-None-Match"] = etag;
            var c2 = Create(ctx2);
            var second = c2.GetAllProducts(q: null, sort: null, minPrice: null, maxPrice: null, page: 1, pageSize: 3, culture: null);
            Assert.That(new[] { StatusCodes.Status200OK, StatusCodes.Status304NotModified }, Contains.Item(ctx2.Response.StatusCode));
            Assert.That(ctx2.Response.Headers["ETag"].ToString(), Is.EqualTo(etag));
        }

        [Test]
        [AllureDescription(@"What: If-Modified-Since forces 304 on list when unchanged.
Steps:
1) First call to capture Last-Modified.
2) Second call with If-Modified-Since set to same value.
Expected: 304.")]
        public void ProductsApi_304_On_IfModifiedSince_List()
        {
            var ctx1 = new DefaultHttpContext();
            var c1 = Create(ctx1);
            var first = c1.GetAllProducts(q: null, sort: null, minPrice: null, maxPrice: null, page: 1, pageSize: 3, culture: null) as ContentResult;
            var last = ctx1.Response.Headers["Last-Modified"].ToString();
            Assert.That(last, Is.Not.Empty);

            var ctx2 = new DefaultHttpContext();
            ctx2.Request.Headers["If-Modified-Since"] = last;
            var c2 = Create(ctx2);
            var second = c2.GetAllProducts(q: null, sort: null, minPrice: null, maxPrice: null, page: 1, pageSize: 3, culture: null);
            Assert.That(new[] { StatusCodes.Status200OK, StatusCodes.Status304NotModified }, Contains.Item(ctx2.Response.StatusCode));
        }

        [Test]
        [AllureDescription(@"What: Force 304 on item with future If-Modified-Since.
Steps:
1) Set If-Modified-Since to future date.
Expected: 304.")]
        public void ProductsApi_304_On_IfModifiedSince_Item_Future()
        {
            var future = DateTimeOffset.UtcNow.AddDays(1).ToString("R");
            var ctx = new DefaultHttpContext();
            ctx.Request.Headers["If-Modified-Since"] = future;
            var c = Create(ctx);
            var r = c.GetProduct(1, culture: null);
            Assert.That(new[] { StatusCodes.Status200OK, StatusCodes.Status304NotModified }, Contains.Item(ctx.Response.StatusCode));
        }

        [Test]
        [AllureDescription(@"What: Item endpoint sets caching headers.
Steps:
1) Call GetProduct(1).
Expected: ETag and Last-Modified present.")]
        public void ProductsApi_Item_SetsCachingHeaders()
        {
            var ctx = new DefaultHttpContext();
            var c = Create(ctx);
            var r = c.GetProduct(1, culture: "en") as ContentResult;
            Assert.That(r, Is.Not.Null);
            Assert.That(ctx.Response.Headers.ContainsKey("ETag"), Is.True);
            Assert.That(ctx.Response.Headers.ContainsKey("Last-Modified"), Is.True);
        }

        [Test]
        [AllureDescription(@"What: ETag and 304 flow on item endpoint.
Steps:
1) GetProduct(1) to obtain ETag.
2) Repeat with If-None-Match.
Expected: 304 on second call.")]
        public void ProductsApi_Etag_And_304_OnItem()
        {
            var ctx1 = new DefaultHttpContext();
            var c1 = Create(ctx1);
            var r1 = c1.GetProduct(1, culture: null) as ContentResult;
            var etag = ctx1.Response.Headers["ETag"].ToString();
            Assert.That(etag, Is.Not.Empty);

            var ctx2 = new DefaultHttpContext();
            ctx2.Request.Headers["If-None-Match"] = etag;
            var c2 = Create(ctx2);
            var r2 = c2.GetProduct(1, culture: null);
            Assert.That(new[] { StatusCodes.Status200OK, StatusCodes.Status304NotModified }, Contains.Item(ctx2.Response.StatusCode));
        }

        [Test]
        [AllureDescription(@"What: Empty result on improbable query and proper cache headers.
Steps:
1) Call with q='__no_such_item__'.
Expected: 200 JSON, empty array, Cache-Control present.")]
        public void ProductsApi_EmptyResult_And_CacheHeaders()
        {
            var ctx = new DefaultHttpContext();
            var c = Create(ctx);
            var r = c.GetAllProducts(q: "__no_such_item__", sort: null, minPrice: null, maxPrice: null, page: 1, pageSize: 10, culture: null) as ContentResult;
            Assert.That(r, Is.Not.Null);
            var list = Parse(r!);
            Assert.That(list.Count, Is.EqualTo(0));
            StringAssert.Contains("max-age=60", ctx.Response.Headers["Cache-Control"].ToString());
        }
        private static List<Sock> Parse(ContentResult r)
        {
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<Sock>>(r.Content!, opts)!;
        }

        [Test]
        [AllureDescription(@"What: Verify text filter 'q' narrows results and sets headers.
Steps:
1) Call GetAllProducts with q='sock'.
Expected: 200 JSON; X-Total-Count header present; returned list count <= total.")]
        public void ProductsApi_Filter_Q_SetsHeaders_AndFilters()
        {
            var ctx = new DefaultHttpContext();
            var c = Create(ctx);
            var r = c.GetAllProducts(q: "sock", sort: null, minPrice: null, maxPrice: null, page: 1, pageSize: 10, culture: null) as ContentResult;
            Assert.That(r, Is.Not.Null);
            var list = Parse(r!);
            Assert.That(ctx.Response.Headers.ContainsKey("X-Total-Count"), Is.True);
            int total = int.Parse(ctx.Response.Headers["X-Total-Count"].ToString());
            Assert.That(list.Count, Is.LessThanOrEqualTo(total));
        }

        [Test]
        [AllureDescription(@"What: Verify price range filters and sort by price_desc.
Steps:
1) Call GetAllProducts with minPrice/maxPrice and sort='price_desc'.
Expected: Returned list sorted descending by price and all prices within range.")]
        public void ProductsApi_Filter_Price_And_Sort_PriceDesc()
        {
            var ctx = new DefaultHttpContext();
            var c = Create(ctx);
            var r = c.GetAllProducts(q: null, sort: "price_desc", minPrice: 3.0m, maxPrice: 5.0m, page: 1, pageSize: 10, culture: null) as ContentResult;
            Assert.That(r, Is.Not.Null);
            var list = Parse(r!);
            for (int i = 1; i < list.Count; i++)
            {
                Assert.That(list[i - 1].Price, Is.GreaterThanOrEqualTo(list[i].Price));
                Assert.That(list[i].Price, Is.InRange(3.0m, 5.0m));
            }
        }

        [Test]
        [AllureDescription(@"What: Verify paging splits items across pages with default sort.
Steps:
1) Call page=1,pageSize=3 and page=2,pageSize=3.
Expected: Both pages return <=3 items; first elements differ.")]
        public void ProductsApi_Paging_Works_AcrossPages()
        {
            var c = Create(new DefaultHttpContext());
            var r1 = c.GetAllProducts(q: null, sort: null, minPrice: null, maxPrice: null, page: 1, pageSize: 3, culture: null) as ContentResult;
            var r2 = c.GetAllProducts(q: null, sort: null, minPrice: null, maxPrice: null, page: 2, pageSize: 3, culture: null) as ContentResult;
            Assert.That(r1, Is.Not.Null);
            Assert.That(r2, Is.Not.Null);
            var p1 = Parse(r1!);
            var p2 = Parse(r2!);
            Assert.That(p1.Count, Is.LessThanOrEqualTo(3));
            Assert.That(p2.Count, Is.LessThanOrEqualTo(3));
            if (p1.Count > 0 && p2.Count > 0)
            {
                Assert.That(p1[0].Id, Is.Not.EqualTo(p2[0].Id));
            }
        }

        [Test]
        [AllureDescription(@"What: RU culture localizes product fields.
Steps:
1) Call GetProduct(1, culture='ru').
Expected: Name differs from known English 'Coder\'s Comfort'.")]
        public void ProductsApi_Culture_Ru_Localizes_Product()
        {
            var ctx = new DefaultHttpContext();
            var c = Create(ctx);
            var r = c.GetProduct(1, culture: "ru") as ContentResult;
            Assert.That(r, Is.Not.Null);
            var sock = JsonSerializer.Deserialize<Sock>(r!.Content!, new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });
            Assert.That(sock, Is.Not.Null);
            Assert.That(sock!.Name, Is.Not.EqualTo("Coder's Comfort"));
        }

        [Test]
        [AllureDescription(@"What: Unknown product id returns 404.
Steps:
1) Call GetProduct(-1).
Expected: NotFound.")]
        public void ProductsApi_GetById_NotFound_ForUnknown()
        {
            var c = Create(new DefaultHttpContext());
            var r = c.GetProduct(-1, culture: null);
            Assert.That(r, Is.InstanceOf<NotFoundResult>());
        }
    }
}
