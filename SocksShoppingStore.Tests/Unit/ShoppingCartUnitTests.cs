using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Models;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("Unit")]
    [AllureEpic("Store")]
    [AllureSuite("Unit Tests")]
    [AllureFeature("Cart")]
    [AllureLabel("package", "SocksShoppingStore.Tests.Unit")]
    [AllureLabel("area", "Unit")]
    [AllureLabel("type", "Functional")]
    [AllureLabel("flow", "Positive")]
    [Category("Unit")]
    [Category("Positive")]
    public class ShoppingCartUnitTests
    {
        [Test]
        [AllureDescription(@"What: Setting quantity to zero removes the item from the cart.
Steps:
1) Add one item to cart.
2) SetQuantity(id, 0).
Expected: Item removed; count equals 0.")]
        public void Cart_SetQuantityZero_RemovesItem()
        {
            var cart = new ShoppingCart();
            cart.AddItem(new Sock { Id = 1, Name = "Test", Price = 2.0m });
            Assert.That(cart.GetTotalItems(), Is.EqualTo(1));

            cart.SetQuantity(1, 0);
            Assert.That(cart.Items.Count, Is.EqualTo(0));
        }

        [Test]
        [AllureDescription(@"What: Validate add/remove/delete operations and total calculation.
Steps:
1) Add socks (two of A, one of B).
2) Verify total items and sum.
3) Remove one A; delete B.
Expected: Counts and totals update accordingly.")]
        public void Cart_AddRemoveDelete_CalculatesTotals()
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

