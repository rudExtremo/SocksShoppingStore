using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureLabel("package", "SocksShoppingStore.Tests.UI")]
    [Category("UI-Smoke")]
    [Category("Positive")]
    [AllureEpic("Store")]
    [AllureSuite("UI Tests")]
    [AllureFeature("Cart")]
    [AllureLabel("area", "UI")]
    [AllureLabel("type", "Functional")]
    [AllureLabel("flow", "Positive")]
    [AllureSeverity(SeverityLevel.critical)]
    public class ShoppingCartTests : BaseTest
    {
        [Test]
        [AllureStory("Add item to cart")]
        [AllureDescription("Verifies cart icon counter updates after adding item.")]
        public void AddToCart_SingleItem_UpdatesCartCounter()
        {
            AllureApi.Step("Step 1: Open home page", () =>
            {
                HomePage!.Navigate();
            });

            AllureApi.Step("Step 2: Ensure cart counter equals 0", () =>
            {
                Assert.That(HomePage!.CartItemCountBadge.Text, Is.EqualTo("0"));
            });

            AllureApi.Step("Step 3: Add first product to cart", () =>
            {
                HomePage!.AddFirstProductToCart();
            });

            AllureApi.Step("Step 4: Ensure cart counter equals 1", () =>
            {
                Assert.That(HomePage!.CartItemCountBadge.Text, Is.EqualTo("1"));
            });
        }

        [Test]
        [AllureStory("Cart full workflow")]
        [AllureDescription("End-to-end: add items, validate total, delete items.")]
        [Ignore("Temporarily disabled in CI to stabilize UI test")]
        public void Cart_FullWorkflow_CalculatesTotalCorrectly()
        {
            AllureApi.Step("Step 1: Open site and add two identical items", () =>
            {
                HomePage!.Navigate();
                HomePage.AddFirstProductToCart();
                HomePage.AddFirstProductToCart();
            });

            AllureApi.Step("Step 2: Navigate to Cart", () =>
            {
                HomePage!.GoToCart();
            });

            AllureApi.Step("Step 3: Validate quantity and total sum", () =>
            {
                Assert.That(CartPage!.GetFirstItemQuantity(), Is.EqualTo(2));
                decimal expectedSum = 6.40m;
                Assert.That(CartPage.GetTotalSum(), Is.EqualTo(expectedSum).Within(0.01m));
            });

            AllureApi.Step("Step 4: Delete item and ensure cart is empty", () =>
            {
                CartPage!.DeleteFirstItem();
                Assert.That(CartPage.IsCartEmpty(), Is.True);
            });
        }
    }
}
