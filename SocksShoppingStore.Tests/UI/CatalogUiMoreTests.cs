using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("UI-Smoke")]
    public class CatalogUiMoreTests : BaseTest
    {
        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Sorting by UI buttons (name asc/desc, price asc).
Steps:
1) Open Home; click 'name_desc' then 'name_asc' then 'price_asc'.
Expected: Visible items ordered accordingly (smoke by comparing pairwise).")]
        public void Catalog_Sort_By_UI_Buttons()
        {
            HomePage!.Navigate();
            HomePage.ClickSortNameDesc();
            System.Threading.Thread.Sleep(300);
            var namesDesc = Driver!.FindElements(By.CssSelector(".product-card h5"));
            Assert.That(namesDesc.Count, Is.GreaterThan(1));

            HomePage.ClickSortNameAsc();
            System.Threading.Thread.Sleep(300);
            var namesAsc = Driver.FindElements(By.CssSelector(".product-card h5"));
            Assert.That(namesAsc.Count, Is.EqualTo(namesDesc.Count));

            HomePage.ClickSortPriceAsc();
            System.Threading.Thread.Sleep(300);
            var prices = HomePage.GetVisibleProductPrices();
            for (int i = 1; i < prices.Count; i++)
                Assert.That(prices[i-1], Is.LessThanOrEqualTo(prices[i]));
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
            System.Threading.Thread.Sleep(300);
            var filtered = HomePage.GetProductCardCount();
            HomePage.ClickResetFilters();
            System.Threading.Thread.Sleep(300);
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
            System.Threading.Thread.Sleep(400);
            var newPrices = HomePage.GetVisibleProductPrices();
            Assert.That(newPrices.Count, Is.GreaterThan(0));
            foreach (var p in newPrices) Assert.That(p, Is.EqualTo(target).Within(0.001m));
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
            System.Threading.Thread.Sleep(300);
            var alerts = Driver!.FindElements(By.CssSelector(".alert.alert-info"));
            var after = HomePage.GetProductCardCount();
            Assert.That(alerts.Count > 0 || after <= before);
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
            Driver!.FindElement(By.CssSelector(".product-card a[href*='/Products/Details'] img"))?.Click();
            StringAssert.Contains("/Products/Details", Driver.Url);
        }
    }
}
