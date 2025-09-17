using OpenQA.Selenium;
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
        }
    }
}

