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
        private By CatalogGridBy = By.CssSelector("#catalog-grid");
        private IWebElement CartLink => _driver.FindElement(By.CssSelector("a[href='/Cart']"));
        public IWebElement CartItemCountBadge => _driver.FindElement(By.CssSelector(".badge"));

        // --- Действия на странице ---
        public void Navigate()
        {
            _driver.Navigate().GoToUrl(_baseUrl);
            EnsurePageReady();
        }

        public void AddFirstProductToCart()
        {
            // Ensure we are on home page (AddToCart redirects to Cart)
            _driver.Navigate().GoToUrl(_baseUrl);
            EnsurePageReady();
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
            IWebElement button = wait.Until(ExpectedConditions.ElementToBeClickable(FirstProductAddToCartButtonBy));

            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("arguments[0].click();", button);
        }

        public void GoToCart()
        {
            CartLink.Click();
        }

        private void AcceptCookiesIfPresent()
        {
            try
            {
                // Wait briefly for banner to appear, then accept
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                var accept = _driver.FindElements(By.Id("cookie-accept"));
                if (accept.Count == 0)
                {
                    try { wait.Until(d => d.FindElements(By.Id("cookie-accept")).Count > 0); }
                    catch { /* ignore timeout */ }
                    accept = _driver.FindElements(By.Id("cookie-accept"));
                }
                if (accept.Count > 0)
                {
                    var el = accept[0];
                    if (el.Displayed)
                    {
                        el.Click();
                    }
                }
            }
            catch
            {
                // ignore any errors when attempting to accept cookies
            }
        }

        private void EnsurePageReady()
        {
            // Wait catalog grid present; then handle cookie and small settle delay
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(CatalogGridBy));
            AcceptCookiesIfPresent();
        }
    }
}
