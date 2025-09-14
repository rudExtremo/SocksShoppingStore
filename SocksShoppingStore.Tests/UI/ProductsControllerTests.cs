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
    [AllureSuite("UI Tests")]
    [AllureFeature("Product Catalog")]
    [AllureLabel("package", "SocksShoppingStore.Tests.UI")]
    [AllureLabel("area", "UI")]
    [AllureLabel("type", "Functional")]
    [AllureLabel("flow", "Positive")]
    [AllureSeverity(SeverityLevel.critical)]
    [Category("UI-Smoke")]
    [Category("Unit")]
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

        [Test]
        public void Details_RussianCulture_Localizes_Name()
        {
            var c = new ProductsController();
            c.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            var prev = System.Globalization.CultureInfo.CurrentUICulture;
            try
            {
                System.Globalization.CultureInfo.CurrentUICulture = new System.Globalization.CultureInfo("ru");
                var result = c.Details(1) as ViewResult;
                Assert.That(result, Is.Not.Null);
                var sock = (SocksShoppingStore.Models.Sock)result!.Model!;
                Assert.That(sock.Name, Is.Not.EqualTo("Coder's Comfort"));
            }
            finally
            {
                System.Globalization.CultureInfo.CurrentUICulture = prev;
            }
        }
    }
}

