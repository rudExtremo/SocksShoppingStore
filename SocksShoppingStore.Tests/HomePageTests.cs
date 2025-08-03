// Файл: HomePageTests.cs
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using SocksShoppingStore.Tests.PageObjects; // <-- Добавляем нашу папку

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    public class HomePageTests
    {
        private IWebDriver? _driver; // Добавили '?'
        private HomePage _homePage; // <-- Поле для нашего Page Object

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            // Создаем экземпляр Page Object, передавая ему драйвер
            _homePage = new HomePage(_driver);
        }

        [Test]
        public void HomePage_Title_IsCorrect()
        {
            // 1. Переходим на страницу (используя метод из Page Object)
            _homePage.Navigate();

            // 2. Ожидаемый заголовок
            string expectedTitle = "Каталог Носков";

            // 3. Получаем фактический заголовок (тоже из Page Object)
            string actualTitle = _homePage.GetPageTitle();

            // 4. Проверка осталась той же
            Assert.That(actualTitle, Does.StartWith(expectedTitle), "Заголовок главной страницы неверный!");
        }
        [Test]
        public void AddToCart_WhenClicked_ProductAppearsInCart()
        {
            // 1. Переходим на главную
            _homePage.Navigate();

            // 2. Кликаем на кнопку "В корзину" у первого товара
            _homePage.ClickAddToCartButtonForFirstProduct();

            // 3. Создаем объект страницы корзины, так как мы ожидаем, что нас на нее перенаправит
            var cartPage = new CartPage(_driver!);

            // 4. Ожидаемое имя первого товара
            string expectedProductName = "Носки 'Программист'";

            // 5. Проверяем, что на странице корзины есть сообщение с именем нашего товара
            Assert.That(cartPage.ContainsProduct(expectedProductName), Is.True, "Товар не был добавлен в корзину!");
        }

        [TearDown]
        public void Teardown()
        {
            _driver?.Quit(); // Добавили '?'
        }
    }
}