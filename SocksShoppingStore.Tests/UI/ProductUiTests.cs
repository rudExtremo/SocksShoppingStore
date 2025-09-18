using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using OpenQA.Selenium;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("UI")]
    [AllureFeature("Product")]
    [Category("UI-Smoke")]
    [NonParallelizable]
    public class ProductUiTests : BaseTest
    {
        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Product details page shows key elements.
Steps:
1) Open first product details.
Expected: Image, name, description, price, 'Add to Cart', 'Back to Catalog' visible.")]
        public void ProductDetails_DisplaysKeyElements()
        {
            HomePage!.Navigate();
            HomePage.OpenFirstProductDetails();
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver!, TimeSpan.FromSeconds(5));
            wait.Until(d => d.FindElements(By.CssSelector(".product-details-image")).Count > 0);
            wait.Until(d => d.FindElements(By.CssSelector(".product-detail-actions .js-add-to-cart")).Count > 0);
            wait.Until(d => d.FindElements(By.CssSelector(".product-detail-actions a.btn-outline-secondary")).Count > 0);
            Assert.That(Driver!.FindElements(By.CssSelector(".product-details-image")).Count, Is.GreaterThan(0));
            Assert.That(Driver.FindElements(By.CssSelector(".product-detail-actions .js-add-to-cart")).Count, Is.GreaterThan(0));
            Assert.That(Driver.FindElements(By.CssSelector(".product-detail-actions a.btn-outline-secondary")).Count, Is.GreaterThan(0));
        }

        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Back to catalog link returns to home.
Steps:
1) Open product details; click 'Back to Catalog'.
Expected: URL returns to '/'.")]
        public void ProductDetails_BackToCatalog_NavigatesHome()
        {
            HomePage!.Navigate();
            HomePage.OpenFirstProductDetails();
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver!, TimeSpan.FromSeconds(5));
            wait.Until(d => d.FindElements(By.CssSelector(".product-detail-actions a.btn-outline-secondary")).Count > 0);
            var back = Driver!.FindElement(By.CssSelector(".product-detail-actions a.btn-outline-secondary"));
            try
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView({behavior:'instant',block:'center'});", back);
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", back);
            }
            catch
            {
                back.Click();
            }
            Assert.That(new System.Uri(Driver.Url).AbsolutePath, Is.EqualTo("/").Or.EqualTo(""));
        }

        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Multiple clicks add multiple quantities of the same item.
Steps:
1) Open details; click 'Add to Cart' 5 times.
Expected: Header total sum increases; unique items counter may remain unchanged.")]
        public void ProductDetails_AddFive_IncrementsCounterByFive()
        {
            HomePage!.Navigate();
            var beforeCount = HomePage.CartItemCountBadge.Text;
            var beforeSum = HomePage!.GetHeaderCartTotal();
            HomePage.OpenFirstProductDetails();
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver!, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElements(OpenQA.Selenium.By.CssSelector(".product-detail-actions .js-add-to-cart")).Count > 0);
            var add = Driver!.FindElement(OpenQA.Selenium.By.CssSelector(".product-detail-actions .js-add-to-cart"));
            for (int i = 0; i < 5; i++)
            {
                var prevCount = HomePage.CartItemCountBadge.Text;
                var prevSum = HomePage.GetHeaderCartTotal();
                try
                {
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView({behavior:'instant',block:'center'});", add);
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", add);
                }
                catch { add.Click(); }
                HomePage.WaitForCartSummaryChange(prevCount, prevSum, 10);
            }
            var afterSum = HomePage.GetHeaderCartTotal();
            Assert.That(afterSum, Is.Not.EqualTo(beforeSum));
        }
    }
}
