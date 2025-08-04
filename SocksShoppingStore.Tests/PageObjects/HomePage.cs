// Файл: PageObjects/HomePage.cs
using OpenQA.Selenium;

namespace SocksShoppingStore.Tests.PageObjects
{
    public class HomePage
    {
        private readonly IWebDriver _driver;
        // URL главной страницы, который мы будем открывать
        private readonly string _baseUrl = "http://127.0.0.1:5123";
        private IWebElement FirstProductAddToCartButton => _driver.FindElement(By.CssSelector(".card .btn-primary"));
        public HomePage(IWebDriver driver)
        {
            _driver = driver;
        }

        // Метод для навигации на страницу
        public void Navigate()
        {
            _driver.Navigate().GoToUrl(_baseUrl);
        }

        // Метод, который возвращает заголовок страницы
        public string GetPageTitle()
        {
            return _driver.Title;
        }
        // Метод, который добавляет товар в корзину
        public void ClickAddToCartButtonForFirstProduct()
        {
            // Получаем доступ к исполнителю JavaScript
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;

            // Выполняем скрипт, который кликает по нашему элементу
            js.ExecuteScript("arguments[0].click();", FirstProductAddToCartButton);
        }
    }
}