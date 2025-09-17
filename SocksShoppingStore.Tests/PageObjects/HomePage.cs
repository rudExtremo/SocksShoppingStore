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
        private By FirstProductAddToCartButtonBy = By.CssSelector(".product-card .js-add-to-cart");
        private IWebElement CartLink => _driver.FindElement(By.CssSelector("a[href='/Cart']"));
        public IWebElement CartItemCountBadge => _driver.FindElement(By.CssSelector(".cart-count"));

        // --- Действия на странице ---
        public void Navigate()
        {
            _driver.Navigate().GoToUrl(_baseUrl);
            AcceptCookiesIfPresent();
        }

        public void NavigateWithQuery(string queryString)
        {
            var url = _baseUrl;
            if (!string.IsNullOrEmpty(queryString))
            {
                url = _baseUrl.TrimEnd('/') + "/" + (queryString.StartsWith("?") ? queryString : ("?" + queryString));
            }
            _driver.Navigate().GoToUrl(url);
            AcceptCookiesIfPresent();
        }

        public void AddFirstProductToCart()
        {
            // Ensure we are on home page (AddToCart redirects back to the page)
            _driver.Navigate().GoToUrl(_baseUrl);
            AcceptCookiesIfPresent();
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
            var button = wait.Until(ExpectedConditions.ElementToBeClickable(FirstProductAddToCartButtonBy));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", button);
        }

        public void GoToCart()
        {
            CartLink.Click();
        }

        public int GetProductCardCount()
        {
            return _driver.FindElements(By.CssSelector(".product-card")).Count;
        }

        public IReadOnlyList<decimal> GetVisibleProductPrices()
        {
            var list = new List<decimal>();
            var priceEls = _driver.FindElements(By.CssSelector(".product-card .card-text strong"));
            foreach (var el in priceEls)
            {
                var text = el.Text.Replace("€", "").Replace("?", "").Trim();
                if (decimal.TryParse(text, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.GetCultureInfo("fr-FR"), out var val))
                {
                    list.Add(val);
                }
            }
            return list;
        }

        public void ClickLoadMoreIfPresentAndWait()
        {
            var btns = _driver.FindElements(By.Id("load-more"));
            if (btns.Count == 0) return;
            var btn = btns[0];
            var before = GetProductCardCount();
            try
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView({behavior:'instant',block:'center'});", btn);
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", btn);
            }
            catch
            {
                btn.Click();
            }
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(_ => GetProductCardCount() > before);
        }

        public void OpenFirstProductDetails()
        {
            var link = _driver.FindElement(By.CssSelector(".product-card a[href*='/Products/Details']"));
            link.Click();
        }

        private void AcceptCookiesIfPresent()
        {
            try
            {
                var banner = _driver.FindElements(By.Id("cookie-consent"));
                if (banner.Count == 0) return;
                var accept = _driver.FindElements(By.Id("cookie-accept"));
                if (accept.Count > 0 && accept[0].Displayed)
                {
                    accept[0].Click();
                }
            }
            catch
            {
                // ignore any errors when attempting to accept cookies
            }
        }
    }
}
