using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
    public class LegalAndLocalizationUnitTests
    {
        [Test]
        [AllureDescription(@"What: Verify Legal.Terms populates ViewData from options.
Steps:
1) Instantiate LegalController with options.
2) Call Terms().
Expected: ViewData contains configured ControllerName and ContactEmail.")]
        public void Legal_Terms_SetsViewData_FromOptions()
        {
            var opts = Options.Create(new LegalOptions { ControllerName = "TestApp", ContactEmail = "t@e.com" });
            var c = new LegalController(opts);
            c.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            var r = c.Terms() as ViewResult;
            Assert.That(r, Is.Not.Null);
            Assert.That(c.ViewData["ControllerName"], Is.EqualTo("TestApp"));
            Assert.That(c.ViewData["ContactEmail"], Is.EqualTo("t@e.com"));
        }

        [Test]
        [AllureDescription(@"What: Verify Localization.Set writes culture cookie and local redirects.
Steps:
1) Call Set('ru-RU','/').
Expected: Response Set-Cookie contains .AspNetCore.Culture; LocalRedirect result.")]
        public void Localization_Set_WritesCookie_AndRedirects_Local()
        {
            var c = new LocalizationController();
            var ctx = new DefaultHttpContext();
            c.ControllerContext = new ControllerContext { HttpContext = ctx };
            // Provide UrlHelper for IsLocalUrl/Action
            c.Url = new SocksShoppingStore.Tests.TestDoubles.TestUrlHelper(c.ControllerContext);
            var r = c.Set("ru-RU", "/") as LocalRedirectResult;
            Assert.That(r, Is.Not.Null);
            StringAssert.Contains(".AspNetCore.Culture", ctx.Response.Headers["Set-Cookie"].ToString());
        }

        [Test]
        [AllureDescription(@"What: Non-local returnUrl should fallback to Home/Index.
Steps:
1) Call Set('en','http://evil.com').
Expected: LocalRedirect to '/'.")]
        public void Localization_Set_NonLocalUrl_FallsBackToHome()
        {
            var c = new LocalizationController();
            var ctx = new DefaultHttpContext();
            c.ControllerContext = new ControllerContext { HttpContext = ctx };
            c.Url = new SocksShoppingStore.Tests.TestDoubles.TestUrlHelper(c.ControllerContext);
            var r = c.Set("en", "http://evil.com") as LocalRedirectResult;
            Assert.That(r, Is.Not.Null);
            Assert.That(r!.Url, Is.EqualTo("/"));
        }
    }
}
