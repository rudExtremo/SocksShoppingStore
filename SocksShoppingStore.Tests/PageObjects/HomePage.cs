using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SocksShoppingStore.Tests.PageObjects
{
    public class HomePage
    {
        private readonly IWebDriver _driver;
        private readonly string _baseUrl;

        public HomePage(IWebDriver driver, string baseUrl)
        {
            _driver = driver;
            _baseUrl = baseUrl;
        }

        // --- Элементы страницы ---
        private By FirstProductAddToCartButtonBy = By.CssSelector(".card .btn-primary");
        private IWebElement CartLink => _driver.FindElement(By.CssSelector("a[href='/Cart']"));
        public IWebElement CartItemCountBadge => _driver.FindElement(By.CssSelector(".badge"));

        // --- Действия на странице ---
        public void Navigate()
        {
            _driver.Navigate().GoToUrl(_baseUrl);
        }

        public void AddFirstProductToCart()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var button = wait.Until(ExpectedConditions.ElementToBeClickable(FirstProductAddToCartButtonBy));

            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("arguments[0].click();", button);
        }

        public void GoToCart()
        {
            CartLink.Click();
        }
    }
}