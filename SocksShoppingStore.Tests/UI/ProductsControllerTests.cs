using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Allure.NUnit;
using SocksShoppingStore.Controllers;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("UI-Smoke")]
    [Category("Positive")]
    public class ProductsControllerTests
    {
        [Test]
        public void Details_NotFound_WhenMissing()
        {
            var c = new ProductsController();
            var result = c.Details(-1);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public void Details_ReturnsView_WithSockModel()
        {
            var c = new ProductsController();
            c.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            var result = c.Details(1) as ViewResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Model, Is.InstanceOf<SocksShoppingStore.Models.Sock>());
        }
    }
}

