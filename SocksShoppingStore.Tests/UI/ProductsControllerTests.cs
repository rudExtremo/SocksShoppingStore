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
    [AllureSuite("UI")]
    [AllureFeature("ProductsController")]
    [Category("UI-Smoke")]
    [Category("Positive")]
    public class ProductsControllerTests
    {
        [Test]
        [AllureDescription(@"What: Verify 'Details' returns NotFound for a missing product.
Steps:
1) Call Details with invalid id (-1).
Expected: NotFoundResult.")]
        public void Products_Details_ReturnsNotFound_WhenMissing()
        {
            var c = new ProductsController();
            var result = c.Details(-1);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        [AllureDescription(@"What: Verify 'Details' returns a view with a Sock model for a valid id.
Steps:
1) Call Details with id=1.
Expected: ViewResult with model of type Sock.")]
        public void Products_Details_ReturnsView_WithSockModel()
        {
            var c = new ProductsController();
            c.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            var result = c.Details(1) as ViewResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Model, Is.InstanceOf<SocksShoppingStore.Models.Sock>());
        }
    }
}

