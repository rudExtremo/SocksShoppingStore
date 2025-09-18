using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Globalization;

namespace SocksShoppingStore.Tests.PageObjects
{
    public class CartPage
    {
        private readonly IWebDriver _driver;

        public CartPage(IWebDriver driver)
        {
            _driver = driver;
        }

        // Locators
        private IWebElement FirstItemQuantityInput => _driver.FindElement(By.CssSelector("tbody tr:first-child .cart-qty-input"));
        private IWebElement TotalSumElement => _driver.FindElement(By.CssSelector("#cart-total-sum"));
        private IWebElement EmptyCartMessage => _driver.FindElement(By.CssSelector(".alert.alert-info"));
        private IWebElement FirstItemDeleteButton => _driver.FindElement(By.CssSelector("tbody tr:first-child .actions-cell a.btn-outline-danger"));

        // Actions / Queries
        public int GetFirstItemQuantity()
        {
            return int.Parse(FirstItemQuantityInput.GetAttribute("value"));
        }

        public void SetFirstItemQuantity(int q)
        {
            var input = FirstItemQuantityInput;
            input.Clear();
            input.SendKeys(q.ToString());
            input.SendKeys(OpenQA.Selenium.Keys.Tab);
        }

        public void ClickIncFirstItem()
        {
            var before = GetFirstItemQuantity();
            var inc = _driver.FindElement(By.CssSelector("tbody tr:first-child a[data-action='inc']"));
            inc.Click();
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(_ => GetFirstItemQuantity() > before);
        }

        public void ClickDecFirstItem()
        {
            var before = GetFirstItemQuantity();
            var dec = _driver.FindElement(By.CssSelector("tbody tr:first-child a[data-action='dec']"));
            dec.Click();
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(_ => GetFirstItemQuantity() < before);
        }

        public decimal GetTotalSum()
        {
            // Parse like "€3,50" using fr-FR (comma decimals)
            string totalText = TotalSumElement.Text.Replace("€", "").Replace("?", "").Trim();
            return decimal.Parse(totalText, CultureInfo.GetCultureInfo("fr-FR"));
        }

        public bool IsCartEmpty()
        {
            return EmptyCartMessage.Displayed;
        }

        public void DeleteFirstItem()
        {
            FirstItemDeleteButton.Click();
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(_ =>
            {
                var inputs = _driver.FindElements(By.CssSelector("tbody .cart-qty-input"));
                var alerts = _driver.FindElements(By.CssSelector(".alert.alert-info"));
                return inputs.Count == 0 || (alerts.Count > 0 && alerts[0].Displayed);
            });
        }
    }
}
