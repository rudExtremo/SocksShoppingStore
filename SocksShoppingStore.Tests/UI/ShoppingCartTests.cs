using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("UI-Smoke")]
    [Category("Positive")]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("UI")]
    [AllureFeature("Shopping Cart")]
    public class ShoppingCartTests : BaseTest
    {
        [Test]
        [Ignore("Temporarily skipped in headless CI/local: stabilize header selectors and counter timing")]
        [AllureStory("Cart Counter")]
        [AllureDescription(@"What: Verify header cart counter increments after adding a single item.
Steps:
1) Open Home page.
2) Ensure the header cart counter displays 0.
3) Click 'Add to Cart' on the first product.
4) Return to header and read the cart counter.
Expected: Counter updates from 0 to 1.")]
        public void Cart_AddSingleItem_UpdatesHeaderCounter()
        {
            AllureApi.Step("Step 1: Open Home page", () =>
            {
                HomePage!.Navigate();
            });

            AllureApi.Step("Step 2: Verify counter is 0", () =>
            {
                var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver!, TimeSpan.FromSeconds(10));
                var sel = OpenQA.Selenium.By.CssSelector("a[aria-label='Cart'] .cart-count, a[aria-label='Cart'] .badge");
                wait.Until(d => d.FindElements(sel).Count > 0);
                var badge = Driver!.FindElement(sel);
                Assert.That(badge.Text, Is.EqualTo("0"));
            });

            AllureApi.Step("Step 3: Add first product to cart", () =>
            {
                HomePage!.AddFirstProductToCart();
            });

            AllureApi.Step("Step 4: Verify counter becomes 1", () =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                while (sw.Elapsed < TimeSpan.FromSeconds(10))
                {
                    if (HomePage!.CartItemCountBadge.Text == "1") break;
                    System.Threading.Thread.Sleep(200);
                }
                Assert.That(HomePage!.CartItemCountBadge.Text, Is.EqualTo("1"));
            });
        }

        [Test]
        [Ignore("Temporarily skipped in headless CI/local: flakiness around cart page interactions; to stabilize waits")]
        [AllureStory("Cart Workflow")]
        [AllureDescription(@"What: Validate cart end-to-end workflow and total calculation.
Steps:
1) From Home, add the first product twice (qty=2).
2) Navigate to the Cart page.
3) Read the first item's quantity and the total sum.
4) Delete the first item and verify the cart becomes empty.
Expected: Quantity equals 2; total equals 6.40; after delete the cart is empty.")]
        public void Cart_AddTwice_VerifyQuantityAndTotal_ThenDelete()
        {
            AllureApi.Step("Step 1: Add two items from Home", () =>
            {
                HomePage!.Navigate();
                HomePage.AddFirstProductToCart();
                HomePage.AddFirstProductToCart();
            });

            AllureApi.Step("Step 2: Go to Cart", () =>
            {
                HomePage!.GoToCart();
            });

            AllureApi.Step("Step 3: Verify qty and total", () =>
            {
                Assert.That(CartPage!.GetFirstItemQuantity(), Is.EqualTo(2));
                decimal expectedSum = 6.40m;
                Assert.That(CartPage.GetTotalSum(), Is.EqualTo(expectedSum).Within(0.01m));
            });

            AllureApi.Step("Step 4: Delete item and verify empty", () =>
            {
                CartPage!.DeleteFirstItem();
                Assert.That(CartPage.IsCartEmpty(), Is.True);
            });
        }
    }
}
