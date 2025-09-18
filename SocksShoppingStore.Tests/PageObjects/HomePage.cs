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
        private IWebElement CartLink => _driver.FindElement(By.CssSelector("a[aria-label='Cart']"));
        public IWebElement CartItemCountBadge
        {
            get
            {
                var sel = By.CssSelector("a[aria-label='Cart'] .cart-count, a[aria-label='Cart'] .badge");
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.FindElements(sel).Count > 0);
                return _driver.FindElement(sel);
            }
        }

        // --- Действия на странице ---
        public void Navigate()
        {
            _driver.Navigate().GoToUrl(_baseUrl);
            AcceptCookiesIfPresent();
            // ensure header cart link is present before proceeding
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElements(By.CssSelector("a[aria-label='Cart']")).Count > 0);
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

            var beforeCount = CartItemCountBadge.Text;
            var beforeTotal = GetHeaderCartTotal();
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
            // Wait until at least one AddToCart button is present and displayed
            wait.Until(d => d.FindElements(FirstProductAddToCartButtonBy).Any(e => e.Displayed && e.Enabled));

            var button = _driver.FindElements(FirstProductAddToCartButtonBy).First(e => e.Displayed && e.Enabled);
            try
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView({behavior:'instant',block:'center'});", button);
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", button);
            }
            catch
            {
                button.Click();
            }

            // Wait for header cart summary to reflect the change
            WaitForCartSummaryChange(beforeCount, beforeTotal, 10);
        }

        public void GoToCart()
        {
            AcceptCookiesIfPresent();
            var link = CartLink;
            try
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView({behavior:'instant',block:'center'});", link);
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", link);
            }
            catch
            {
                link.Click();
            }
            // Wait for cart page to load: either table present or empty alert
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(_ =>
            {
                var table = _driver.FindElements(By.CssSelector("#cart-table .cart-qty-input"));
                var alert = _driver.FindElements(By.CssSelector(".alert.alert-info"));
                return table.Count > 0 || alert.Count > 0;
            });
        }

        public int GetProductCardCount()
        {
            return _driver.FindElements(By.CssSelector(".product-card")).Count;
        }

        public IReadOnlyList<decimal> GetVisibleProductPrices()
        {
            // Be resilient to DOM updates during read (stale elements)
            var attempts = 0;
            while (attempts < 3)
            {
                try
                {
                    var list = new List<decimal>();
                    var priceEls = _driver.FindElements(By.CssSelector(".product-card .card-text strong"));
                    foreach (var el in priceEls)
                    {
                        string text;
                        try { text = el.Text; }
                        catch (OpenQA.Selenium.StaleElementReferenceException) { continue; }
                        var m = System.Text.RegularExpressions.Regex.Match(text, "\\d+[,.]\\d{2}");
                        if (!m.Success) continue;
                        var num = m.Value.Replace('.', ',');
                        if (decimal.TryParse(num, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.GetCultureInfo("fr-FR"), out var val)) list.Add(val);
                    }
                    return list;
                }
                catch (OpenQA.Selenium.StaleElementReferenceException)
                {
                    attempts++;
                    System.Threading.Thread.Sleep(100);
                }
            }
            // Last resort: empty list
            return Array.Empty<decimal>();
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
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElements(By.CssSelector(".product-card a[href*='/Products/Details']")).Count > 0);
            var link = _driver.FindElement(By.CssSelector(".product-card a[href*='/Products/Details']"));
            link.Click();
        }

        public string GetHeaderCartTotal()
        {
            var nav = _driver.FindElement(By.CssSelector("a[aria-label='Cart']"));
            var el = nav.FindElements(By.CssSelector(".cart-total, .ms-1.text-muted"));
            return el.Count > 0 ? el[0].Text : string.Empty;
        }

        public void WaitForCartSummaryChange(string previousCountText, string previousTotalText, int timeoutSeconds = 5)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutSeconds));
            wait.Until(_ =>
            {
                try
                {
                    var cnt = CartItemCountBadge.Text;
                    var tot = GetHeaderCartTotal();
                    return cnt != previousCountText || tot != previousTotalText;
                }
                catch
                {
                    return false;
                }
            });
        }

        // Filters and sorting helpers
        public void SetPriceFilter(string? min, string? max)
        {
            var minInput = _driver.FindElement(By.CssSelector("input[name='minPrice']"));
            var maxInput = _driver.FindElement(By.CssSelector("input[name='maxPrice']"));
            minInput.Clear(); if (!string.IsNullOrEmpty(min)) minInput.SendKeys(min);
            maxInput.Clear(); if (!string.IsNullOrEmpty(max)) maxInput.SendKeys(max);
        }

        public void ClickApplyFilters()
        {
            _driver.FindElement(By.CssSelector("button.btn-filter[type='submit']")).Click();
        }

        public void ClickResetFilters()
        {
            _driver.FindElement(By.CssSelector("a.btn.btn-outline-secondary.btn-filter"))?.Click();
        }

        public void ClickSortPriceAsc() => _driver.FindElement(By.CssSelector("a.btn-sort[href*='sort=price_asc']")).Click();
        public void ClickSortPriceDesc() => _driver.FindElement(By.CssSelector("a.btn-sort[href*='sort=price_desc']")).Click();
        public void ClickSortNameAsc() => _driver.FindElement(By.CssSelector("a.btn-sort[href*='sort=name_asc']")).Click();
        public void ClickSortNameDesc() => _driver.FindElement(By.CssSelector("a.btn-sort[href*='sort=name_desc']")).Click();

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
