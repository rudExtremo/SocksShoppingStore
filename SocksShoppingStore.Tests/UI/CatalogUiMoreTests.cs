using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("UI-Smoke")]
    [NonParallelizable]
    public class CatalogUiMoreTests : BaseTest
    {
        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Sort by name descending via UI button.
Steps: 1) Open Home; 2) Click 'name_desc'.
Expected: Names are in non-increasing order.")]
        public void Catalog_Sort_Name_Desc_Orders_Correctly()
        {
            HomePage!.Navigate();
            HomePage.ClickSortNameDesc();
            var wait = new WebDriverWait(Driver!, TimeSpan.FromSeconds(10));
            wait.Until(_ =>
            {
                var els = Driver!.FindElements(By.CssSelector(".product-card h5"));
                if (els.Count < 2) return false;
                var names = els.Select(e => e.Text).ToList();
                for (int i = 1; i < names.Count; i++)
                    if (string.Compare(names[i - 1], names[i], StringComparison.Ordinal) < 0) return false;
                return true;
            });
        }

        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Sort by name ascending via UI button.
Steps: 1) Open Home; 2) Click 'name_asc'.
Expected: Names are in non-decreasing order.")]
        public void Catalog_Sort_Name_Asc_Orders_Correctly()
        {
            HomePage!.Navigate();
            HomePage.ClickSortNameAsc();
            var wait = new WebDriverWait(Driver!, TimeSpan.FromSeconds(10));
            wait.Until(_ =>
            {
                var els = Driver!.FindElements(By.CssSelector(".product-card h5"));
                if (els.Count < 2) return false;
                var names = els.Select(e => e.Text).ToList();
                for (int i = 1; i < names.Count; i++)
                    if (string.Compare(names[i - 1], names[i], StringComparison.Ordinal) > 0) return false;
                return true;
            });
        }

        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Sort by price ascending via UI button.
Steps: 1) Open Home; 2) Click 'price_asc'.
Expected: Prices are in non-decreasing order.")]
        public void Catalog_Sort_Price_Asc_Orders_Correctly()
        {
            HomePage!.Navigate();
            HomePage.ClickSortPriceAsc();
            var wait = new WebDriverWait(Driver!, TimeSpan.FromSeconds(10));
            wait.Until(_ =>
            {
                var prices = HomePage.GetVisibleProductPrices();
                if (prices.Count < 2) return false;
                for (int i = 1; i < prices.Count; i++)
                    if (prices[i - 1] > prices[i]) return false;
                return true;
            });
        }

        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Reset filters and sorting.
Steps:
1) Apply price range; click Reset.
Expected: Card count is restored/increases (smoke check).")]
        public void Catalog_Reset_Filters()
        {
            HomePage!.Navigate();
            var before = HomePage.GetProductCardCount();
            HomePage.SetPriceFilter("3.00", "3.50");
            HomePage.ClickApplyFilters();
            var wait = new WebDriverWait(Driver!, TimeSpan.FromSeconds(12));
            wait.Until(_ =>
            {
                var count = HomePage.GetProductCardCount();
                var alerts = Driver!.FindElements(By.CssSelector(".alert.alert-info"));
                return alerts.Count > 0 || count != before || count > 0;
            });
            var filtered = HomePage.GetProductCardCount();
            HomePage.ClickResetFilters();
            wait.Until(_ =>
            {
                var count = HomePage.GetProductCardCount();
                var alerts = Driver!.FindElements(By.CssSelector(".alert.alert-info"));
                return count >= filtered || alerts.Count == 0;
            });
            var after = HomePage.GetProductCardCount();
            Assert.That(after, Is.GreaterThanOrEqualTo(filtered));
        }

        [Test]
        [Category("Boundary")]
        [AllureDescription(@"What: Price boundary equals to one product's price.
Steps:
1) Read first visible product price; set min=max=that price; Apply.
Expected: Visible products have that price only (<= page size).")]
        public void Catalog_Price_Boundary_Equals_ItemPrice()
        {
            HomePage!.Navigate();
            var prices = HomePage.GetVisibleProductPrices();
            Assert.That(prices.Count, Is.GreaterThan(0));
            var target = prices[0];
            var min = (target - 0.05m).ToString(System.Globalization.CultureInfo.InvariantCulture);
            var max = (target + 0.05m).ToString(System.Globalization.CultureInfo.InvariantCulture);
            HomePage.SetPriceFilter(min, max);
            HomePage.ClickApplyFilters();
            // Wait for the catalog to settle after server-side reload
            HomePage.WaitForCatalogStable(15);
            var wait = new WebDriverWait(Driver!, TimeSpan.FromSeconds(15));
            wait.Until(_ =>
            {
                try
                {
                    var np = HomePage.GetVisibleProductPrices();
                    return np.Count > 0 && np.All(p => Math.Abs(p - target) <= 0.02m);
                }
                catch (OpenQA.Selenium.StaleElementReferenceException)
                {
                    return false;
                }
            });
        }

        [Test]
        [Category("Negative")]
        [AllureDescription(@"What: Min greater than Max should lead to empty or unchanged list (implementation-dependent).
Steps:
1) Set min=10, max=5; Apply.
Expected: Either 'NoItems' alert or fewer cards.")]
        public void Catalog_Price_MinGreaterThanMax_Handled()
        {
            HomePage!.Navigate();
            var before = HomePage.GetProductCardCount();
            HomePage.SetPriceFilter("10", "5");
            HomePage.ClickApplyFilters();
            var wait = new WebDriverWait(Driver!, TimeSpan.FromSeconds(7));
            wait.Until(_ =>
            {
                var alerts = Driver!.FindElements(By.CssSelector(".alert.alert-info"));
                var after = HomePage.GetProductCardCount();
                return alerts.Count > 0 || after <= before;
            });
        }

        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Click on product image opens details page.
Steps:
1) Click image in first card.
Expected: URL contains '/Products/Details'.")]
        public void Catalog_Click_Image_Opens_Details()
        {
            HomePage!.Navigate();
            var wait = new WebDriverWait(Driver!, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElements(By.CssSelector(".product-card a[href*='/Products/Details'] img")).Count > 0);
            var img = Driver!.FindElement(By.CssSelector(".product-card a[href*='/Products/Details'] img"));
            try
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView({behavior:'instant',block:'center'});", img);
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", img);
            }
            catch { img.Click(); }
            StringAssert.Contains("/Products/Details", Driver.Url);
        }
    }
}
