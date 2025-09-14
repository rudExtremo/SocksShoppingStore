using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Models;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("Магазин")]
    [AllureSuite("Юнит-тесты")]
    [AllureFeature("Корзина")]
    [AllureLabel("package", "SocksShoppingStore.Tests.Unit")]
    [Category("Unit")]
    [Category("Positive")]
    public class ShoppingCartUnitTests
    {
        [Test]
        public void SetQuantity_Zero_RemovesItem()
        {
            var cart = new ShoppingCart();
            cart.AddItem(new Sock { Id = 1, Name = "Test", Price = 2.0m });
            Assert.That(cart.GetTotalItems(), Is.EqualTo(1));

            cart.SetQuantity(1, 0);
            Assert.That(cart.Items.Count, Is.EqualTo(0));
        }

        [Test]
        public void Add_Remove_Delete_CalculateTotals()
        {
            var cart = new ShoppingCart();
            var s1 = new Sock { Id = 1, Name = "A", Price = 3.0m };
            var s2 = new Sock { Id = 2, Name = "B", Price = 2.5m };

            cart.AddItem(s1);
            cart.AddItem(s1);
            cart.AddItem(s2);

            Assert.That(cart.GetTotalItems(), Is.EqualTo(3));
            Assert.That(cart.GetTotalSum(), Is.EqualTo(3.0m*2 + 2.5m));

            cart.RemoveItem(1);
            Assert.That(cart.GetTotalItems(), Is.EqualTo(2));

            cart.DeleteItem(2);
            Assert.That(cart.GetTotalItems(), Is.EqualTo(1));
        }
    }
}
