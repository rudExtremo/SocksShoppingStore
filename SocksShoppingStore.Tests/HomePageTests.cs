using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SocksShoppingStore.Tests.PageObjects;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    public class HomePageTests
    {
        private IWebDriver driver;
        private const string BaseUrl = "http://localhost:5123";

        // Этот метод выполнится один раз перед всеми тестами в этом классе
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Создаем экземпляр драйвера только один раз
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
        }

        // Этот метод будет выполняться перед каждым тестом
        [SetUp]
        public void Setup()
        {
            // Очищаем куки перед каждым тестом для обеспечения полной изоляции
            driver.Manage().Cookies.DeleteAllCookies();
            // Переходим на главную страницу, чтобы каждый тест начинался с чистого состояния
            driver.Navigate().GoToUrl(BaseUrl);
        }

        // Этот метод выполнится один раз после всех тестов в этом классе
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // Закрываем драйвер только один раз в конце
            driver.Quit();
        }

        [Test]
        public void AddItemToCart_ShouldIncreaseCartCounter()
        {
            var homePage = new HomePage(driver);
            string initialCartCount = homePage.GetCartCount();
            Assert.That(initialCartCount, Is.EqualTo("0"), "Initial cart count should be 0");

            // Добавляем первый товар в корзину
            homePage.AddFirstItemToCart();

            // Небольшое ожидание, чтобы счетчик успел обновиться
            System.Threading.Thread.Sleep(500);

            string updatedCartCount = homePage.GetCartCount();
            Assert.That(updatedCartCount, Is.EqualTo("1"), "Cart count should be 1 after adding an item");
        }

        [Test]
        public void NavigateToCart_ShouldOpenCartPage()
        {
            var homePage = new HomePage(driver);

            // Кликаем на иконку корзины
            CartPage cartPage = homePage.ClickCartIcon();

            // Проверяем, что мы на странице корзины, проверяя заголовок
            Assert.That(cartPage.GetPageTitle(), Is.EqualTo("Cart"), "Should be navigated to the Cart page");
        }
    }
}
