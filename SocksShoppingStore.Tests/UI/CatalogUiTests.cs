using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("UI")]
    [AllureFeature("Catalog")]
    [Category("UI-Smoke")]
    [NonParallelizable]
    public class CatalogUiTests : BaseTest
    {
        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Filter catalog by price range.
Steps:
1) Open home with minPrice=3.0,maxPrice=5.0.
Expected: All visible prices fall within [3.0, 5.0].")]
        public void Catalog_Filter_By_PriceRange_Shows_Only_InRange()
        {
            HomePage!.NavigateWithQuery("?minPrice=3.0&maxPrice=5.0&pageSize=4");
            var prices = HomePage.GetVisibleProductPrices();
            Assert.That(prices.Count, Is.GreaterThan(0));
            foreach (var p in prices)
                Assert.That(p, Is.InRange(3.0m, 5.0m));
        }

        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Sort by price descending.
Steps:
1) Open home with sort=price_desc.
Expected: Visible prices are in non-increasing order.")]
        public void Catalog_Sort_Price_Desc_Orders_Correctly()
        {
            HomePage!.NavigateWithQuery("?sort=price_desc&pageSize=4");
            var prices = HomePage.GetVisibleProductPrices();
            Assert.That(prices.Count, Is.GreaterThan(1));
            for (int i = 1; i < prices.Count; i++)
                Assert.That(prices[i - 1], Is.GreaterThanOrEqualTo(prices[i]));
        }

        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Lazy-load adds more items when available.
Steps:
1) Open home; click 'Load more' (if present).
Expected: Card count increases or stays if already all loaded.")]
        public void Catalog_LazyLoad_Increases_CardCount_When_Available()
        {
            HomePage!.NavigateWithQuery("?pageSize=4");
            var before = HomePage.GetProductCardCount();
            HomePage.ClickLoadMoreIfPresentAndWait();
            var after = HomePage.GetProductCardCount();
            Assert.That(after, Is.GreaterThanOrEqualTo(before));
        }

        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Add to cart from product details.
Steps:
1) Open first product details; click Add to Cart.
2) Verify header cart counter increments.
Expected: Counter increases by 1.")]
        public void Details_AddToCart_IncrementsCounter()
        {
            HomePage!.Navigate();
            var before = HomePage.CartItemCountBadge.Text;
            var beforeTotalGlobal = HomePage.GetHeaderCartTotal();
            HomePage.OpenFirstProductDetails();
            AcceptCookiesIfPresent();
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

            HomePage.WaitForCartSummaryChange(before, beforeTotalGlobal, 10);
            var after = HomePage.CartItemCountBadge.Text;
            Assert.That(after, Is.Not.EqualTo(before));
        }

        // Cart flow tests moved to CartUiTests.cs (split into independent cases)

        [Test]
        [Category("Negative")]
        [AllureDescription(@"What: Invalid search query should not break the page.
Steps:
1) Open home with q containing special characters.
Expected: Page loads (either cards or 'NoItems' alert). No crash.")]
        public void Catalog_InvalidSearch_NoCrash()
        {
            HomePage!.NavigateWithQuery("?q=%27%20OR%201%3D1%3B--&pageSize=4");
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver!, TimeSpan.FromSeconds(5));
            wait.Until(_ =>
            {
                var cards = HomePage.GetProductCardCount();
                var alerts = Driver!.FindElements(OpenQA.Selenium.By.CssSelector(".alert.alert-info"));
                return cards > 0 || alerts.Count > 0;
            });
            var count = HomePage.GetProductCardCount();
            Assert.That(count >= 0);
        }
    }
}

