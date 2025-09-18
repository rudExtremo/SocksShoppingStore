using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Controllers;
using SocksShoppingStore.Tests.TestDoubles;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("UI")]
    [AllureFeature("HomeController")]
    [Category("UI-Smoke")]
    [Category("Positive")]
    public class HomeControllerTests
    {
        private static HomeController Create()
        {
            var c = new HomeController();
            c.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { Session = new TestSession() } };
            return c;
        }

        [Test]
        [AllureDescription(@"What: Verify default Index returns a paged catalog model.
Steps:
1) Call Index with page=1,pageSize=3.
Expected: ViewResult with model having 3 items, total>=3, page=1.")]
        public void Home_Index_Default_ReturnsPagedModel()
        {
            var controller = Create();
            var result = controller.Index(q: null, sort: null, minPrice: null, maxPrice: null, page: 1, pageSize: 3) as ViewResult;
            Assert.That(result, Is.Not.Null);
            var model = result!.Model as SocksShoppingStore.Models.CatalogViewModel;
            Assert.That(model, Is.Not.Null);
            Assert.That(model!.Items.Count, Is.EqualTo(3));
            Assert.That(model.Total, Is.GreaterThanOrEqualTo(3));
            Assert.That(model.Page, Is.EqualTo(1));
        }

        [Test]
        [AllureDescription(@"What: Verify search/filter with price range sorts by price_desc.
Steps:
1) Call Index with q='socks', sort='price_desc', minPrice=3.5, maxPrice=5.0.
Expected: Items are sorted by price descending.")]
        public void Home_Index_SearchAndFilter_SortsByPriceDesc()
        {
            var controller = Create();
            var result = controller.Index(q: "socks", sort: "price_desc", minPrice: 3.5m, maxPrice: 5.0m, page: 1, pageSize: 10) as ViewResult;
            Assert.That(result, Is.Not.Null);
            var model = (SocksShoppingStore.Models.CatalogViewModel)result!.Model!;
            var items = model.Items;
            for (int i = 1; i < items.Count; i++)
            {
                Assert.That(items[i-1].Price, Is.GreaterThanOrEqualTo(items[i].Price));
            }
        }
    }
}

