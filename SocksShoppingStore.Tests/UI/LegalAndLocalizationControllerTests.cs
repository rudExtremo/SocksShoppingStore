using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Controllers;
using SocksShoppingStore.Tests.TestDoubles;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("UI-Smoke")]
    [Category("Positive")]
    public class LegalAndLocalizationControllerTests
    {
        [Test]
        [AllureDescription(@"What: Verify Privacy view receives ViewData values from options.
Steps:
1) Instantiate LegalController with options.
2) Call Privacy().
Expected: ViewData contains configured ControllerName and ContactEmail.")]
        public void Legal_Privacy_SetsViewData_FromOptions()
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
        [AllureDescription(@"What: Verify Localization.Set writes culture cookie and redirects locally.
Steps:
1) Call Set('ru','/').
Expected: Response 'Set-Cookie' contains .AspNetCore.Culture; LocalRedirect result.")]
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
    }
}
