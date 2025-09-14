using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Allure.Net.Commons;
using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Controllers;
using SocksShoppingStore.Tests.TestDoubles;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("Store")]
    [AllureSuite("UI Tests")]
    [AllureFeature("Localization")]
    [AllureLabel("package", "SocksShoppingStore.Tests.UI")]
    [AllureLabel("area", "UI")]
    [AllureLabel("type", "Functional")]
    [AllureLabel("flow", "Positive")]
    [AllureSeverity(SeverityLevel.critical)]
    [Category("UI-Smoke")]
    [Category("Unit")]
    [Category("Positive")]
    public class LegalAndLocalizationControllerTests
    {
        [Test]
        public void Legal_Privacy_Sets_ViewData_FromOptions()
        {
            var opts = Options.Create(new LegalOptions { ControllerName = "TestApp", ContactEmail = "test@example.com" });
            var controller = new LegalController(opts);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            var view = controller.Privacy() as ViewResult;
            Assert.That(view, Is.Not.Null);
            Assert.That(view!.ViewData["ControllerName"], Is.EqualTo("TestApp"));
            Assert.That(view!.ViewData["ContactEmail"], Is.EqualTo("test@example.com"));
        }

        [Test]
        public void Legal_Terms_Sets_ViewData_FromOptions()
        {
            var opts = Options.Create(new LegalOptions { ControllerName = "TestApp", ContactEmail = "test@example.com" });
            var controller = new LegalController(opts);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            var view = controller.Terms() as ViewResult;
            Assert.That(view, Is.Not.Null);
            Assert.That(view!.ViewData["ControllerName"], Is.EqualTo("TestApp"));
            Assert.That(view!.ViewData["ContactEmail"], Is.EqualTo("test@example.com"));
        }

        [Test]
        public void Localization_Set_WritesCookie_AndRedirects()
        {
            var controller = new LocalizationController();
            var ctx = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = ctx };

            // Stub UrlHelper so Url.IsLocalUrl/Action are available
            controller.Url = new TestUrlHelper(controller.ControllerContext);

            var r = controller.Set("ru", "/") as LocalRedirectResult;
            Assert.That(r, Is.Not.Null);
            Assert.That(ctx.Response.Headers["Set-Cookie"].ToString(), Does.Contain(".AspNetCore.Culture"));
        }

        [Test]
        public void Localization_Set_InvalidReturnUrl_FallsBack_ToHome()
        {
            var controller = new LocalizationController();
            var ctx = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = ctx };
            controller.Url = new TestUrlHelper(controller.ControllerContext);

            var r = controller.Set("ru-RU", "https://evil.example/") as LocalRedirectResult;
            Assert.That(r, Is.Not.Null);
            // TestUrlHelper.Action returns '/'; fallback should use that
            Assert.That(r!.Url, Is.EqualTo("/"));
        }
    }
}
