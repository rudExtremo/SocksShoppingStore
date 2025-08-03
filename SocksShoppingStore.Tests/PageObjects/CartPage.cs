// Файл: PageObjects/CartPage.cs
using OpenQA.Selenium;

namespace SocksShoppingStore.Tests.PageObjects
{
    public class CartPage
    {
        private readonly IWebDriver _driver;

        public CartPage(IWebDriver driver)
        {
            _driver = driver;
        }

        // Находим элемент с сообщением об успехе
        private IWebElement SuccessMessage => _driver.FindElement(By.CssSelector(".alert-success"));

        // Метод для проверки, содержит ли сообщение об успехе имя товара
        public bool ContainsProduct(string productName)
        {
            return SuccessMessage.Text.Contains(productName);
        }
    }
}