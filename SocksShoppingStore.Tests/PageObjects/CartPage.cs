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

        // --- Элементы страницы ---
        private IWebElement FirstItemQuantity => _driver.FindElement(By.XPath("//tbody/tr[1]/td[3]//span"));
        private IWebElement TotalSumElement => _driver.FindElement(By.XPath("//tfoot//strong[contains(text(),'€')]"));
        private IWebElement EmptyCartMessage => _driver.FindElement(By.CssSelector(".alert-info"));
        private IWebElement FirstItemDeleteButton => _driver.FindElement(By.XPath("//tbody/tr[1]/td[5]/a"));

        // --- Действия и проверки ---
        public int GetFirstItemQuantity()
        {
            return int.Parse(FirstItemQuantity.Text);
        }

        public decimal GetTotalSum()
        {
            // Убираем символ валюты и пробелы, чтобы получить чистое число
            string totalText = TotalSumElement.Text.Replace("€", "").Trim();
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
