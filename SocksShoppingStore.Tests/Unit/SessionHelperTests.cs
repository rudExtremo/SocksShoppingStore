using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Helpers;
using SocksShoppingStore.Models;
using SocksShoppingStore.Tests.TestDoubles;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("Unit")]
    [Category("Positive")]
    public class SessionHelperTests
    {
        [Test]
        [AllureDescription(@"What: Verify session Set/Get helpers serialize ShoppingCart.
Steps:
1) Create cart with two items; session.Set(""Cart"", cart).
2) session.Get<ShoppingCart>(""Cart"").
Expected: Not null; total items equals 2.")]
        public void SessionHelpers_SetAndGet_SerializesShoppingCart()
        {
            ISession session = new TestSession();
            var cart = new ShoppingCart();
            cart.AddItem(new Sock { Id = 1, Name = "A", Price = 1.2m });
            cart.AddItem(new Sock { Id = 2, Name = "B", Price = 3.4m });

            session.Set("Cart", cart);
            var loaded = session.Get<ShoppingCart>("Cart");

            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded!.GetTotalItems(), Is.EqualTo(2));
        }
    }
}
