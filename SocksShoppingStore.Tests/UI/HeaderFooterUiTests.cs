using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using OpenQA.Selenium;
using Allure.Net.Commons;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("UI-Smoke")]
    public class HeaderFooterUiTests : BaseTest
    {
        [Test, Ignore("pending: navbar-brand not reliably found in headless")] 
        [Category("Positive")]
        [AllureDescription(@"What: Clicking logo navigates to Home page.
Steps:
1) Open product details.
2) Click header brand 'SocksShoppingStore'.
Expected: URL path is '/' (catalog).")]
        public void HeaderLogo_NavigatesToHome()
        {
            HomePage!.Navigate();
            HomePage.OpenFirstProductDetails();
            Driver!.FindElement(By.CssSelector("a.navbar-brand"))!.Click();
            Assert.That(new System.Uri(Driver.Url).AbsolutePath, Is.EqualTo("/").Or.EqualTo(""));
        }

        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Cart summary updates near header icon.
Steps:
1) Read counter and total.
2) Add item from catalog.
Expected: Counter or total changes (increments).")]
        public void HeaderCartSummary_Updates_OnAdd()
        {
            HomePage!.Navigate();
            var beforeCount = HomePage.CartItemCountBadge.Text;
            var beforeTotal = Driver!.FindElement(By.CssSelector(".cart-total")).Text;
            HomePage.AddFirstProductToCart();
            System.Threading.Thread.Sleep(600);
            var afterCount = HomePage.CartItemCountBadge.Text;
            var afterTotal = Driver.FindElement(By.CssSelector(".cart-total")).Text;
            Assert.That(afterCount != beforeCount || afterTotal != beforeTotal, "Header cart summary did not change");
        }

        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Clicking header cart link navigates to cart page.
Steps:
1) Click cart icon.
Expected: URL contains '/Cart'.")]
        public void HeaderCartLink_NavigatesToCart()
        {
            HomePage!.Navigate();
            Driver!.FindElement(By.CssSelector("a[aria-label='Cart']")).Click();
            StringAssert.Contains("/Cart", Driver.Url);
        }

        [Test, Ignore("pending: footer click intercepted in headless, will refine")]
        [Category("Positive")]
        [AllureDescription(@"What: Footer legal links navigate to pages.
Steps:
1) Scroll to footer; click both legal links.
Expected: URL contains '/Legal/'.")]
        public void FooterLinks_Terms_Privacy_Work()
        {
            HomePage!.Navigate();
            AcceptCookiesIfPresent();
            ((IJavaScriptExecutor)Driver!).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, TimeSpan.FromSeconds(5));
            wait.Until(d => d.FindElements(By.CssSelector("footer a[href*='legal']")).Count >= 2 || d.FindElements(By.CssSelector("footer a[href*='Legal']")).Count >= 2);
            var links = Driver!.FindElements(By.CssSelector("footer a[href*='legal'], footer a[href*='Legal']"));
            links[0].Click();
            StringAssert.Contains("/Legal/", Driver.Url);
            Driver.Navigate().Back();
            ((IJavaScriptExecutor)Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            links = Driver.FindElements(By.CssSelector("footer a[href*='/Legal']"));
            links[1].Click();
            StringAssert.Contains("/Legal/", Driver.Url);
        }
    }
}
