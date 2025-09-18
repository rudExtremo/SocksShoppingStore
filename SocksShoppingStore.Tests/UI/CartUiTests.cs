using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("UI")]
    [AllureFeature("Cart")]
    [Category("UI-Smoke")]
    [NonParallelizable]
    public class CartUiTests : BaseTest
    {
        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Increment quantity via '+' button.
Steps:
1) Add item; go to cart.
2) Click '+' and observe quantity.
Expected: Quantity increases by 1.")]
        public void Cart_Increment_Quantity_Increases_By_One()
        {
            HomePage!.Navigate();
            HomePage.AddFirstProductToCart();
            HomePage.GoToCart();

            var q1 = CartPage!.GetFirstItemQuantity();
            CartPage.ClickIncFirstItem();
            var q2 = CartPage.GetFirstItemQuantity();
            Assert.That(q2, Is.EqualTo(q1 + 1));
        }

        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Decrement quantity via '-' button.
Steps:
1) Add two items; go to cart.
2) Click '-' and observe quantity.
Expected: Quantity decreases by 1.")]
        public void Cart_Decrement_Quantity_Decreases_By_One()
        {
            HomePage!.Navigate();
            HomePage.AddFirstProductToCart();
            HomePage.AddFirstProductToCart();
            HomePage.GoToCart();

            var q1 = CartPage!.GetFirstItemQuantity();
            CartPage.ClickDecFirstItem();
            var q2 = CartPage.GetFirstItemQuantity();
            Assert.That(q2, Is.EqualTo(q1 - 1));
        }

        [Test]
        [Category("Boundary")]
        [AllureDescription(@"What: Setting 0 clamps to 1 (client-side rule).
Steps:
1) Add item; go to cart.
2) Set quantity to 0 and blur.
Expected: Quantity becomes at least 1.")]
        public void Cart_SetQuantity_Zero_Clamps_To_One()
        {
            HomePage!.Navigate();
            HomePage.AddFirstProductToCart();
            HomePage.GoToCart();

            CartPage!.SetFirstItemQuantity(0);
            // Re-read with internal waits from page object paths
            var q = CartPage.GetFirstItemQuantity();
            Assert.That(q, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Deleting item empties cart when single item present.
Steps:
1) Add item; go to cart.
2) Click trash button.
Expected: Cart becomes empty (info alert visible).")]
        public void Cart_Delete_Removes_Item_And_Empty_State()
        {
            HomePage!.Navigate();
            HomePage.AddFirstProductToCart();
            HomePage.GoToCart();

            CartPage!.DeleteFirstItem();
            Assert.That(CartPage.IsCartEmpty(), Is.True);
        }
    }
}
