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
    [Category("Unit")]
    [Category("Positive")]
    public class ProductsAndHomeUnitTests
    {
        [Test]
        [AllureDescription(@"What: ProductsController.Details returns NotFound for missing id and view for valid id.
Steps:
1) Call Details(-1) -> NotFound.
2) Call Details(1) -> ViewResult with model.")]
        public void Products_Details_NotFound_And_View()
        {
            var c = new ProductsController();
            var nf = c.Details(-1);
            Assert.That(nf, Is.InstanceOf<NotFoundResult>());

            c.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            var ok = c.Details(1) as ViewResult;
            Assert.That(ok, Is.Not.Null);
            Assert.That(ok!.Model, Is.Not.Null);
        }

        [Test]
        [AllureDescription(@"What: Home.Privacy returns a view.
Steps:
1) Call Privacy().
Expected: ViewResult.")]
        public void Home_Privacy_ReturnsView()
        {
            var c = new HomeController();
            var r = c.Privacy();
            Assert.That(r, Is.InstanceOf<ViewResult>());
        }
    }
}

