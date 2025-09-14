using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;
using SocksShoppingStore.Controllers;
using SocksShoppingStore.Tests.TestDoubles;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("Store")]
    [AllureSuite("UI Tests")]
    [AllureFeature("Home")]
    [AllureLabel("package", "SocksShoppingStore.Tests.UI")]
    [AllureLabel("area", "UI")]
    [AllureLabel("type", "Functional")]
    [AllureLabel("flow", "Positive")]
    [AllureSeverity(SeverityLevel.critical)]
    [Category("UI-Smoke")]
    [Category("Unit")]
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
        public void Index_Default_ReturnsPagedModel()
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
        public void Privacy_Returns_View()
        {
            var controller = Create();
            var result = controller.Privacy() as ViewResult;
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Index_SearchAndFilter_SortsByPriceDesc()
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

        [Test]
        public void Index_InvalidPaging_IsClamped_And_Defaults_WhenZero()
        {
            var controller = Create();
            // page < 1 and pageSize <= 0 should clamp to page=1, pageSize=6 (default)
            var result = controller.Index(q: null, sort: "name_desc", minPrice: null, maxPrice: null, page: -5, pageSize: 0) as ViewResult;
            Assert.That(result, Is.Not.Null);
            var model = (SocksShoppingStore.Models.CatalogViewModel)result!.Model!;
            Assert.That(model.Page, Is.EqualTo(1));
            Assert.That(model.PageSize, Is.EqualTo(6));
            // name_desc sorting check (first >= second lexicographically)
            var items = model.Items;
            if (items.Count >= 2)
            {
                Assert.That(string.Compare(items[0].Name, items[1].Name, StringComparison.Ordinal) >= 0);
            }
        }

        [Test]
        public void Index_PriceAsc_SortsAscending_WhenOnlyMinPriceApplied()
        {
            var controller = Create();
            var result = controller.Index(q: null, sort: "price_asc", minPrice: 2.5m, maxPrice: null, page: 1, pageSize: 8) as ViewResult;
            Assert.That(result, Is.Not.Null);
            var model = (SocksShoppingStore.Models.CatalogViewModel)result!.Model!;
            var items = model.Items;
            for (int i = 1; i < items.Count; i++)
            {
                Assert.That(items[i-1].Price, Is.LessThanOrEqualTo(items[i].Price));
            }
        }
    }
}

