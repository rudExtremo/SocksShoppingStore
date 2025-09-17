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
    [Category("UI-Regression")]
    [Category("Positive")]
    public class CheckoutControllerTests
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
        [AllureDescription(@"What: Verify GET /Checkout returns a view with model.
Steps:
1) Invoke Index().
Expected: ViewResult with CheckoutViewModel.")]
        public void Checkout_GetIndex_ReturnsView()
        {
            var (c, _) = Create();
            var r = c.Index() as ViewResult;
            Assert.That(r, Is.Not.Null);
            Assert.That(r!.Model, Is.InstanceOf<CheckoutViewModel>());
        }

        [Test]
        [AllureDescription(@"What: Verify POST /Checkout with invalid model returns view with errors.
Steps:
1) Prepare invalid model and add ModelState error.
2) Post Index(model).
Expected: ViewResult; ModelState invalid.")]
        public void Checkout_PostIndex_InvalidModel_ReturnsViewWithErrors()
        {
            var (c, ctx) = Create();
            // Cart empty -> ModelState error
            var vm = new CheckoutViewModel { CustomerName = "", Email = "bad" };
            c.ModelState.AddModelError("CustomerName", "Required");
            var r = c.Index(vm) as ViewResult;
            Assert.That(r, Is.Not.Null);
            Assert.That(c.ModelState.IsValid, Is.False);
        }

        [Test]
        [AllureDescription(@"What: Verify valid POST /Checkout stores order draft and redirects to Review.
Steps:
1) Put one item into the session cart.
2) Post valid Index(model).
Expected: Redirect to Review; 'OrderDraft' present in session with 1 item.")]
        public void Checkout_PostIndex_Valid_SetsOrderDraft_AndRedirectsToReview()
        {
            var (c, ctx) = Create();
            // Put one item to cart
            var cart = new ShoppingCart();
            cart.AddItem(new Sock { Id = 1, Name = "A", Price = 2 });
            ctx.Session.Set("Cart", cart);

            var vm = new CheckoutViewModel
            {
                CustomerName = "John Doe",
                Email = "john@example.com",
                AddressLine1 = "Street 1",
                City = "City",
                PostalCode = "123",
                Country = "US"
            };
            var r = c.Index(vm) as RedirectToActionResult;
            Assert.That(r, Is.Not.Null);
            Assert.That(r!.ActionName, Is.EqualTo("Review"));
            var draft = ctx.Session.Get<Order>("OrderDraft");
            Assert.That(draft, Is.Not.Null);
            Assert.That(draft!.Items.Count, Is.EqualTo(1));
        }

        [Test]
        [AllureDescription(@"What: Verify Review returns a view when draft exists in session.
Steps:
1) Seed 'OrderDraft' in session.
2) Call Review().
Expected: ViewResult not null.")]
        public void Checkout_Review_WithDraft_ReturnsView()
        {
            var (c, ctx) = Create();
            ctx.Session.Set("OrderDraft", new Order { CustomerName = "X" });
            var r = c.Review() as ViewResult;
            Assert.That(r, Is.Not.Null);
        }

        [Test]
        [AllureDescription(@"What: Verify Confirm finalizes order, clears cart, and redirects to ThankYou.
Steps:
1) Seed cart and 'OrderDraft' in session.
2) Call Confirm().
Expected: Redirect to ThankYou; cart emptied; LastOrder set with total=2.")]
        public void Checkout_Confirm_FinalizesOrder_ClearsCart_AndRedirects()
        {
            var (c, ctx) = Create();
            // Cart with item and draft
            var cart = new ShoppingCart();
            cart.AddItem(new Sock { Id = 1, Name = "A", Price = 2, });
            ctx.Session.Set("Cart", cart);
            ctx.Session.Set("OrderDraft", new Order { CustomerName = "X", Items = new List<OrderItem> { new OrderItem{ ProductId=1, Name="A", UnitPrice=2, Quantity=1 } } });

            var r = c.Confirm() as RedirectToActionResult;
            Assert.That(r, Is.Not.Null);
            Assert.That(r!.ActionName, Is.EqualTo("ThankYou"));

            var cleared = ctx.Session.Get<ShoppingCart>("Cart");
            Assert.That(cleared!.Items.Count, Is.EqualTo(0));
            var last = ctx.Session.Get<Order>("LastOrder");
            Assert.That(last, Is.Not.Null);
            Assert.That(last!.Total, Is.EqualTo(2));
        }
    }
}

