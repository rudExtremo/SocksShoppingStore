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
    [AllureEpic("Store")]
    [AllureSuite("Unit Tests")]
    [AllureFeature("Session")]
    [AllureLabel("package", "SocksShoppingStore.Tests.Unit")]
    [AllureLabel("area", "Unit")]
    [AllureLabel("type", "Functional")]
    [AllureLabel("flow", "Positive")]
    [Category("Unit")]
    [Category("Positive")]
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

