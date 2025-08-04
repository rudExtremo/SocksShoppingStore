using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SocksShoppingStore.Tests.PageObjects;

namespace SocksShoppingStore.Tests
{
    public class BaseTest
    {
        protected IWebDriver? Driver;
        protected HomePage? HomePage;
        protected CartPage? CartPage;

        [SetUp]
        public void Setup()
        {
            // Определяем, запущены ли мы в среде CI (Continuous Integration)
            bool isCi = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));

            var options = new ChromeOptions();
            if (isCi)
            {
                // Настройки для запуска на GitHub Actions
                options.AddArgument("--headless");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--disable-gpu");
            }

            Driver = new ChromeDriver(options);
            Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            // В зависимости от среды, используем разный базовый URL
            // Для локального запуска используем порт 7068 из твоего launchSettings.json
            string baseUrl = isCi ? "http://127.0.0.1:5123" : "https://localhost:7068";

            // Инициализируем Page Objects
            HomePage = new HomePage(Driver, baseUrl);
            CartPage = new CartPage(Driver);
        }

        [TearDown]
        public void Teardown()
        {
            Driver?.Quit();
        }
    }
}