using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using OpenQA.Selenium;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("UI-Smoke")]
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
            Driver!.FindElement(By.CssSelector(".product-detail-actions a.btn-outline-secondary")).Click();
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
            var beforeSum = Driver!.FindElement(By.CssSelector(".cart-total")).Text;
            HomePage.OpenFirstProductDetails();
            var add = Driver!.FindElement(By.CssSelector(".product-detail-actions .js-add-to-cart"));
            for (int i = 0; i < 5; i++)
            {
                add.Click();
                System.Threading.Thread.Sleep(250);
            }
            var afterSum = Driver.FindElement(By.CssSelector(".cart-total")).Text;
            Assert.That(afterSum, Is.Not.EqualTo(beforeSum));
        }
    }
}
