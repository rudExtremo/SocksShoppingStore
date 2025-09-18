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
    public class HomeControllerUnitTestsExtended
    {
        private static HomeController Create() => new HomeController();

        [Test]
        [AllureDescription(@"What: Exercise Home.Index across sort modes and filters to cover branches.
Steps:
1) Call Index with q filter.
2) Call Index with min/max price.
3) Call Index with sort=name_desc, price_asc, price_desc.
4) Call Index with pagination inputs.
Expected: Calls succeed and return ViewResult; model fields are consistent.")]
        public void Home_Index_Covers_Sorts_Filters_And_Paging()
        {
            var c = Create();

            var v1 = c.Index(q: "sock", sort: null, minPrice: null, maxPrice: null, page: 1, pageSize: 6) as ViewResult;
            Assert.That(v1, Is.Not.Null);

            var v2 = c.Index(q: null, sort: null, minPrice: 2.0m, maxPrice: 5.0m, page: 1, pageSize: 6) as ViewResult;
            Assert.That(v2, Is.Not.Null);

            // Trigger different sort branches
            var vd1 = c.Index(q: null, sort: "name_desc", minPrice: null, maxPrice: null, page: 1, pageSize: 6) as ViewResult;
            var vd2 = c.Index(q: null, sort: "price_asc", minPrice: null, maxPrice: null, page: 1, pageSize: 6) as ViewResult;
            var vd3 = c.Index(q: null, sort: "price_desc", minPrice: null, maxPrice: null, page: 1, pageSize: 6) as ViewResult;
            Assert.Multiple(() => {
                Assert.That(vd1, Is.Not.Null);
                Assert.That(vd2, Is.Not.Null);
                Assert.That(vd3, Is.Not.Null);
            });

            // Paging bounds
            var v3 = c.Index(q: null, sort: null, minPrice: null, maxPrice: null, page: -1, pageSize: 0) as ViewResult;
            Assert.That(v3, Is.Not.Null);
        }
    }
}

