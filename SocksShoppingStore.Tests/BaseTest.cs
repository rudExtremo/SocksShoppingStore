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
    public class BaseTest
    {
        protected IWebDriver? Driver;
        protected HomePage? HomePage;
        protected CartPage? CartPage;

        [SetUp]
        public void Setup()
        {
            bool runUi = TestSettings.RunUi;
            if (!runUi)
            {
                Assert.Ignore("Skipping UI tests on CI. Set RUN_UI_TESTS=1 to enable.");
                return;
            }

            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            if (TestSettings.IgnoreCertErrors)
            {
                // Allow self-signed dev cert only when explicitly enabled
                options.AddArgument("--ignore-certificate-errors");
                options.AddArgument("--allow-insecure-localhost");
            }

            try
            {
                Driver = new ChromeDriver(options);
            }
            catch (Exception ex)
            {
                if (!runUi)
                {
                    Assert.Ignore($"Skipping UI tests: {ex.GetType().Name}: {ex.Message}");
                    return;
                }
                throw;
            }

            Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            string baseUrl = TestSettings.BaseUrl;

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
