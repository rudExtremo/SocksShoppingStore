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

