using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SocksShoppingStore.Controllers;
using SocksShoppingStore.Tests.TestDoubles;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("API-Smoke")]
    [Category("Positive")]
    public class ApiDependenciesControllersSmokeTests
    {
        [Test]
        [AllureDescription(@"What: HomeController Index basic smoke (API dependency).
Steps:
1) Call Index with default parameters.
Expected: ViewResult with model.")]
        public void HomeController_Index_Smoke()
        {
            var c = new HomeController();
            c.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { Session = new TestSession() } };
            var r = c.Index(q: null, sort: null, minPrice: null, maxPrice: null, page: 1, pageSize: 6) as ViewResult;
            Assert.That(r, Is.Not.Null);
            Assert.That(r!.Model, Is.Not.Null);
        }

        [Test]
        [AllureDescription(@"What: ProductsController.Details returns view for valid id (API dependency).
Steps:
1) Call Details(1).
Expected: ViewResult with model.")]
        public void ProductsController_Details_Smoke()
        {
            var c = new ProductsController();
            c.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            var r = c.Details(1) as ViewResult;
            Assert.That(r, Is.Not.Null);
            Assert.That(r!.Model, Is.Not.Null);
        }

        [Test]
        [AllureDescription(@"What: LegalController Privacy returns view (API dependency).
Steps:
1) Call Privacy().
Expected: ViewResult.")]
        public void LegalController_Privacy_Smoke()
        {
            var opts = Options.Create(new LegalOptions());
            var c = new LegalController(opts);
            c.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            var r = c.Privacy() as ViewResult;
            Assert.That(r, Is.Not.Null);
        }
    }
}

