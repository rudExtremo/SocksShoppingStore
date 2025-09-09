using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SocksShoppingStore.Tests.PageObjects;
using System.IO;

namespace SocksShoppingStore.Tests
{
    [AllureNUnit]
    public class BaseTest
    {
        protected IWebDriver? Driver;
        protected HomePage? HomePage;
        protected CartPage? CartPage;

        [SetUp]
        public void Setup()
        {
            bool isCi = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));
            bool runUi = string.Equals(Environment.GetEnvironmentVariable("RUN_UI_TESTS"), "1", StringComparison.Ordinal);

            if (isCi && !runUi)
            {
                Assert.Ignore("Skipping UI tests on CI. Set RUN_UI_TESTS=1 to enable.");
                return;
            }

            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");

            try
            {
                Driver = new ChromeDriver(options);
            }
            catch (Exception ex)
            {
                if (isCi && !runUi)
                {
                    Assert.Ignore($"Skipping UI tests: {ex.GetType().Name}: {ex.Message}");
                    return;
                }
                throw;
            }

            Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            string baseUrl = isCi ? "http://127.0.0.1:5123" : "https://localhost:7068";

            HomePage = new HomePage(Driver, baseUrl);
            CartPage = new CartPage(Driver);
        }

        [TearDown]
        public void Teardown()
        {
            // Делаем скриншот, если тест упал
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed && Driver is ITakesScreenshot)
            {
                var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
                AllureApi.AddAttachment(
                    "Screenshot on failure",
                    "image/png",
                    screenshot.AsByteArray);
            }

            Driver?.Quit();
        }
    }
}
