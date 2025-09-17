using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Controllers;
using SocksShoppingStore.Tests.TestDoubles;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("Unit")]
    [Category("Positive")]
    public class CartControllerNavigationUnitTests
    {
        private static (CartController ctrl, DefaultHttpContext ctx) Create()
        {
            var ctrl = new CartController();
            var ctx = new DefaultHttpContext { Session = new TestSession() };
            ctrl.ControllerContext = new ControllerContext { HttpContext = ctx };
            return (ctrl, ctx);
        }

        [Test]
        [AllureDescription(@"What: When returnUrl is local, AddToCart redirects there.
Steps:
1) Call AddToCart with returnUrl='/?q=x'.
Expected: LocalRedirect to '/?q=x'.")]
        public void Cart_AddToCart_LocalReturnUrl_Redirects()
        {
            var (c, ctx) = Create();
            // Provide UrlHelper for IsLocalUrl
            c.Url = new SocksShoppingStore.Tests.TestDoubles.TestUrlHelper(c.ControllerContext);
            var r = c.AddToCart(1, returnUrl: "/?q=x") as LocalRedirectResult;
            Assert.That(r, Is.Not.Null);
            Assert.That(r!.Url, Is.EqualTo("/?q=x"));
        }

        [Test]
        [AllureDescription(@"What: When Referer points to product page, AddToCart redirects back; if from Cart, goes Home.
Steps:
1) Set Referer to '/Products/Details/1' -> LocalRedirect to that path.
2) Set Referer to '/Cart' -> RedirectToAction Home/Index.
Expected: Behaviors as described.")]
        public void Cart_AddToCart_RefererFallback_Behaves()
        {
            var (c1, ctx1) = Create();
            c1.Url = new SocksShoppingStore.Tests.TestDoubles.TestUrlHelper(c1.ControllerContext);
            ctx1.Request.Headers["Referer"] = "http://localhost/Products/Details/1";
            var r1 = c1.AddToCart(1) as LocalRedirectResult;
            Assert.That(r1, Is.Not.Null);
            Assert.That(r1!.Url, Is.EqualTo("/Products/Details/1"));

            var (c2, ctx2) = Create();
            c2.Url = new SocksShoppingStore.Tests.TestDoubles.TestUrlHelper(c2.ControllerContext);
            ctx2.Request.Headers["Referer"] = "http://localhost/Cart";
            var r2 = c2.AddToCart(1) as RedirectToActionResult;
            Assert.That(r2, Is.Not.Null);
            Assert.That(r2!.ActionName, Is.EqualTo("Index"));
            Assert.That(r2!.ControllerName, Is.EqualTo("Home"));
        }
    }
}
