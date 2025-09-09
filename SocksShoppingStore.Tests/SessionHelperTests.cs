using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using SocksShoppingStore.Helpers;
using SocksShoppingStore.Models;
using SocksShoppingStore.Tests.TestDoubles;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class SessionHelperTests
    {
        [Test]
        public void SetAndGet_Serializes_ShoppingCart()
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

