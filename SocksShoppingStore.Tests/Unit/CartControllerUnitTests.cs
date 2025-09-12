using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Allure.NUnit;
using SocksShoppingStore.Controllers;
using SocksShoppingStore.Models;
using SocksShoppingStore.Tests.TestDoubles;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("Unit")]
    public class CartControllerUnitTests
    {
        private static (CartController ctrl, TestSession session) Create()
        {
            var ctrl = new CartController();
            var ctx = new DefaultHttpContext { Session = new TestSession() };
            ctrl.ControllerContext = new ControllerContext { HttpContext = ctx };
            return (ctrl, (TestSession)ctx.Session);
        }

        [Test]
        public void Add_Remove_Delete_Clear_Workflow()
        {
            var (c, session) = Create();

            // Initially empty
            var indexResult = c.Index() as ViewResult;
            var cart = (ShoppingCart)indexResult!.Model!;
            Assert.That(cart.Items.Count, Is.EqualTo(0));

            // Add item id=1 twice
            c.AddToCart(1);
            c.AddToCart(1);

            var view2 = c.Index() as ViewResult;
            var cart2 = (ShoppingCart)view2!.Model!;
            Assert.That(cart2.GetTotalItems(), Is.EqualTo(2));

            // Remove one
            c.RemoveFromCart(1);
            var view3 = c.Index() as ViewResult;
            var cart3 = (ShoppingCart)view3!.Model!;
            Assert.That(cart3.GetTotalItems(), Is.EqualTo(1));

            // Delete item
            c.DeleteItem(1);
            var view4 = c.Index() as ViewResult;
            var cart4 = (ShoppingCart)view4!.Model!;
            Assert.That(cart4.Items.Count, Is.EqualTo(0));

            // Clear
            c.AddToCart(2);
            c.Clear();
            var view5 = c.Index() as ViewResult;
            var cart5 = (ShoppingCart)view5!.Model!;
            Assert.That(cart5.Items.Count, Is.EqualTo(0));
        }

        [Test]
        public void SetQuantity_Zero_Removes()
        {
            var (c, _) = Create();
            c.AddToCart(1);
            var r = c.SetQuantity(1, 0) as RedirectToActionResult;
            Assert.That(r, Is.Not.Null);
            var v = c.Index() as ViewResult;
            var cart = (ShoppingCart)v!.Model!;
            Assert.That(cart.Items.Count, Is.EqualTo(0));
        }
    }
}

