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
        private By FirstProductAddToCartButtonBy = By.CssSelector("#catalog-grid .card .btn.btn-primary");
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
            // Wait until at least one AddToCart button is present
            wait.Until(ExpectedConditions.ElementExists(FirstProductAddToCartButtonBy));
            var button = _driver.FindElement(FirstProductAddToCartButtonBy);
            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", button);
                wait.Until(ExpectedConditions.ElementToBeClickable(button));
                js.ExecuteScript("arguments[0].click();", button);
            }
            catch (NoSuchElementException)
            {
                // Retry once after ensuring page and cookies
                EnsurePageReady();
                var btn2 = _driver.FindElement(FirstProductAddToCartButtonBy);
                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", btn2);
                wait.Until(ExpectedConditions.ElementToBeClickable(btn2));
                js.ExecuteScript("arguments[0].click();", btn2);
            }
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
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            wait.Until(ExpectedConditions.ElementExists(CatalogGridBy));
            AcceptCookiesIfPresent();
        }
    }
}
