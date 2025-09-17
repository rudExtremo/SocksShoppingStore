using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Controllers;
using SocksShoppingStore.Helpers;
using SocksShoppingStore.Models;
using SocksShoppingStore.Tests.TestDoubles;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("Unit")]
    [Category("Positive")]
    public class CheckoutControllerUnitFlowTests
    {
        private static (CheckoutController ctrl, DefaultHttpContext ctx) Create()
        {
            var logger = NullLogger<CheckoutController>.Instance;
            var ctrl = new CheckoutController(logger);
            var ctx = new DefaultHttpContext { Session = new TestSession() };
            ctrl.ControllerContext = new ControllerContext { HttpContext = ctx };
            return (ctrl, ctx);
        }

        [Test]
        [AllureDescription(@"What: Verify GET /Checkout returns a view with default model.
Steps:
1) Call Index().
Expected: ViewResult with CheckoutViewModel.")]
        public void Checkout_GetIndex_ReturnsView()
        {
            var (c, _) = Create();
            var r = c.Index() as ViewResult;
            Assert.That(r, Is.Not.Null);
            Assert.That(r!.Model, Is.InstanceOf<CheckoutViewModel>());
        }

        [Test]
        [AllureDescription(@"What: Posting with honeypot/empty cart yields ModelState errors and returns view.
Steps:
1) Post Index with Website filled or empty cart.
Expected: ModelState invalid; returns View.")]
        public void Checkout_PostIndex_Invalid_ReturnsView()
        {
            var (c, ctx) = Create();
            var vm = new CheckoutViewModel { Website = "bot", CustomerName = "x", Email = "x@example.com" };
            var r = c.Index(vm) as ViewResult;
            Assert.That(r, Is.Not.Null);
            Assert.That(c.ModelState.IsValid, Is.False);
        }

        [Test]
        [AllureDescription(@"What: Valid post stores OrderDraft in session and redirects to Review.
Steps:
1) Put one item to session cart.
2) Post valid model.
Expected: Redirect to Review; 'OrderDraft' exists in session.")]
        public void Checkout_PostIndex_Valid_StoresDraft_AndRedirects()
        {
            var (c, ctx) = Create();
            var cart = new ShoppingCart();
            cart.AddItem(new Sock { Id = 1, Name = "A", Price = 2 });
            ctx.Session.Set("Cart", cart);

            var vm = new CheckoutViewModel
            {
                CustomerName = "John Doe",
                Email = "john@example.com",
                AddressLine1 = "A",
                City = "C",
                PostalCode = "123",
                Country = "US"
            };
            var r = c.Index(vm) as RedirectToActionResult;
            Assert.That(r, Is.Not.Null);
            Assert.That(r!.ActionName, Is.EqualTo("Review"));
            Assert.That(ctx.Session.Get<Order>("OrderDraft"), Is.Not.Null);
        }

        [Test]
        [AllureDescription(@"What: Review returns view when draft exists; otherwise redirects to Index.
Steps:
1) Without draft -> redirect.
2) With draft -> view.
Expected: Behaviors as described.")]
        public void Checkout_Review_Behaves_AsExpected()
        {
            var (c, ctx) = Create();
            var r1 = c.Review();
            Assert.That(r1, Is.InstanceOf<RedirectToActionResult>());

            ctx.Session.Set("OrderDraft", new Order { CustomerName = "X" });
            var r2 = c.Review();
            Assert.That(r2, Is.InstanceOf<ViewResult>());
        }

        [Test]
        [AllureDescription(@"What: Confirm finalizes order, clears cart, clears draft, and redirects to ThankYou.
Steps:
1) Seed draft and cart in session.
2) Call Confirm().
Expected: Redirect to ThankYou; 'LastOrder' set; Cart cleared.")]
        public void Checkout_Confirm_Finalizes_And_Clears()
        {
            var (c, ctx) = Create();
            var cart = new ShoppingCart();
            cart.AddItem(new Sock { Id = 1, Name = "A", Price = 2 });
            ctx.Session.Set("Cart", cart);
            ctx.Session.Set("OrderDraft", new Order { CustomerName = "Y", Items = new List<OrderItem> { new OrderItem{ ProductId=1, Name="A", UnitPrice=2, Quantity=1 } } });

            var r = c.Confirm() as RedirectToActionResult;
            Assert.That(r, Is.Not.Null);
            Assert.That(r!.ActionName, Is.EqualTo("ThankYou"));

            var cleared = ctx.Session.Get<ShoppingCart>("Cart");
            Assert.That(cleared!.Items.Count, Is.EqualTo(0));
            var last = ctx.Session.Get<Order>("LastOrder");
            Assert.That(last, Is.Not.Null);
            Assert.That(last!.Total, Is.GreaterThan(0));
        }
    }
}

