using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;
using SocksShoppingStore.Controllers;
using SocksShoppingStore.Helpers;
using SocksShoppingStore.Models;
using SocksShoppingStore.Tests.TestDoubles;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("Магазин")]
    [AllureSuite("UI Тесты")]
    [AllureFeature("Оформление заказа")]
    [AllureLabel("package", "SocksShoppingStore.Tests.UI")]
    [AllureSeverity(SeverityLevel.critical)]
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
        public void Get_Index_Returns_View()
        {
            var (c, _) = Create();
            var r = c.Index() as ViewResult;
            Assert.That(r, Is.Not.Null);
            Assert.That(r!.Model, Is.InstanceOf<CheckoutViewModel>());
        }

        [Test]
        public void Post_Index_Invalid_Model_Returns_View_WithErrors()
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
        public void Post_Index_Valid_Sets_OrderDraft_And_Redirects_To_Review()
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
        public void Review_With_Draft_Returns_View()
        {
            var (c, ctx) = Create();
            ctx.Session.Set("OrderDraft", new Order { CustomerName = "X" });
            var r = c.Review() as ViewResult;
            Assert.That(r, Is.Not.Null);
        }

        [Test]
        public void Confirm_Finalizes_Order_Clears_Cart_And_Redirects()
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

