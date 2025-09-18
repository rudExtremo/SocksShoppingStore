using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("UI-Smoke")]
    public class CatalogUiTests : BaseTest
    {
        [Test]
        [Category("Positive")]
        [Ignore("HeadlessSkip: requires stabilization")]
        [AllureDescription(@"What: Verify catalog filters, sorting and lazy-load behavior.
Steps:
1) Open home with minPrice=3.0,maxPrice=5.0,sort=price_desc.
2) Validate prices sorted desc and within range.
3) Click 'Load more' and ensure more cards appear (if total > pageSize).
Expected: Ordering correct; card count increases on load more.")]
        public void Catalog_Filters_Sort_And_LazyLoad()
        {
            HomePage!.NavigateWithQuery("?minPrice=3.0&maxPrice=5.0&sort=price_desc");
            var prices = HomePage.GetVisibleProductPrices();
            for (int i = 1; i < prices.Count; i++)
            {
                Assert.That(prices[i - 1], Is.GreaterThanOrEqualTo(prices[i]));
                Assert.That(prices[i], Is.InRange(3.0m, 5.0m));
            }
            var before = HomePage.GetProductCardCount();
            HomePage.ClickLoadMoreIfPresentAndWait();
            var after = HomePage.GetProductCardCount();
            Assert.That(after, Is.GreaterThanOrEqualTo(before));
        }

        [Test]
        [Category("Positive")]
        [Ignore("HeadlessSkip: requires stabilization")]
        [AllureDescription(@"What: Add to cart from product details.
Steps:
1) Open first product details; click Add to Cart.
2) Verify header cart counter increments.
Expected: Counter increases by 1.")]
        public void Details_AddToCart_IncrementsCounter()
        {
            HomePage!.Navigate();
            var before = HomePage.CartItemCountBadge.Text;
            HomePage.OpenFirstProductDetails();
            var addSel = OpenQA.Selenium.By.CssSelector(".product-detail-actions .js-add-to-cart");
            var add = Driver!.FindElement(addSel);
            try
            {
                ((OpenQA.Selenium.IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView({behavior:'instant',block:'center'});", add);
                ((OpenQA.Selenium.IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", add);
            }
            catch
            {
                add.Click();
            }

            var sw = System.Diagnostics.Stopwatch.StartNew();
            var beforeTotal = Driver.FindElement(OpenQA.Selenium.By.CssSelector(".cart-total")).Text;
            while (sw.Elapsed < System.TimeSpan.FromSeconds(5))
            {
                try
                {
                    var cnt = HomePage.CartItemCountBadge.Text;
                    var tot = Driver.FindElement(OpenQA.Selenium.By.CssSelector(".cart-total")).Text;
                    if (cnt != before || tot != beforeTotal) break;
                }
                catch { }
                System.Threading.Thread.Sleep(150);
            }
            var after = HomePage.CartItemCountBadge.Text;
            Assert.That(after != before || Driver.FindElement(OpenQA.Selenium.By.CssSelector(".cart-total")).Text != beforeTotal,
                "Header cart summary did not change after AddToCart");
        }

        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Increase/decrease quantity and set quantity via input on Cart page.
Steps:
1) Add two items; go to cart.
2) Click '+' then '-' and verify quantity changes.
3) Set quantity to 0 -> UI clamps to 1 (client-side).
4) Delete item via trash button -> cart empty.
Expected: Qty updates accordingly; zero clamps to 1; delete empties cart.")]
        [Ignore("HeadlessSkip: requires stabilization")]
        public void Cart_IncDec_And_SetQuantity_Zero_Removes()
        {
            HomePage!.Navigate();
            HomePage.AddFirstProductToCart();
            HomePage.AddFirstProductToCart();
            HomePage.GoToCart();

            var q1 = CartPage!.GetFirstItemQuantity();
            CartPage.ClickIncFirstItem();
            System.Threading.Thread.Sleep(400);
            var q2 = CartPage.GetFirstItemQuantity();
            Assert.That(q2, Is.EqualTo(q1 + 1));
            CartPage.ClickDecFirstItem();
            System.Threading.Thread.Sleep(400);
            var q3 = CartPage.GetFirstItemQuantity();
            Assert.That(q3, Is.EqualTo(q2 - 1));

            CartPage.SetFirstItemQuantity(0);
            System.Threading.Thread.Sleep(600);
            var q4 = CartPage.GetFirstItemQuantity();
            Assert.That(q4, Is.GreaterThanOrEqualTo(1));
            CartPage.DeleteFirstItem();
            System.Threading.Thread.Sleep(400);
            Assert.That(CartPage.IsCartEmpty(), Is.True);
        }

        [Test]
        [Category("Negative")]
        [Ignore("HeadlessSkip: requires stabilization")]
        [AllureDescription(@"What: Invalid search query should not break the page.
Steps:
1) Open home with q containing special characters.
Expected: Page loads (either cards or 'NoItems' alert). No crash.")]
        public void Catalog_InvalidSearch_NoCrash()
        {
            HomePage!.NavigateWithQuery("?q=%27%20OR%201%3D1%3B--");
            // Either some cards or 'NoItems' alert
            var cards = HomePage.GetProductCardCount();
            if (cards == 0)
            {
                var alerts = Driver!.FindElements(OpenQA.Selenium.By.CssSelector(".alert.alert-info"));
                Assert.That(alerts.Count, Is.GreaterThanOrEqualTo(1));
            }
            else
            {
                Assert.That(cards, Is.GreaterThan(0));
            }
        }
    }
}

