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
            options.AddArgument("--window-size=1280,900");
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

            // Warm-up: open base URL once to accept cookies and stabilize header
            try
            {
                Driver.Navigate().GoToUrl(baseUrl);
                AcceptCookiesIfPresent();
            }
            catch { }
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

        protected void AcceptCookiesIfPresent()
        {
            if (Driver == null) return;
            try
            {
                // Ensure DOM is ready
                WaitForDomReady(Driver, TimeSpan.FromSeconds(10));

                var banner = Driver.FindElements(By.Id("cookie-consent"));
                if (banner.Count > 0)
                {
                    var accept = Driver.FindElements(By.Id("cookie-accept"));
                    if (accept.Count > 0 && accept[0].Displayed)
                    {
                        accept[0].Click();
                    }
                    // Wait until banner is gone
                    var end = DateTime.UtcNow + TimeSpan.FromSeconds(5);
                    while (DateTime.UtcNow < end)
                    {
                        banner = Driver.FindElements(By.Id("cookie-consent"));
                        if (banner.Count == 0 || !banner[0].Displayed) break;
                        System.Threading.Thread.Sleep(100);
                    }
                }
                // Ensure header cart link is present
                WaitForHeaderReady(Driver, TimeSpan.FromSeconds(10));
            }
            catch { }
        }

        protected static void WaitForDomReady(IWebDriver driver, TimeSpan timeout)
        {
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, timeout);
            wait.Until(d =>
            {
                try
                {
                    var state = ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState")?.ToString();
                    return string.Equals(state, "complete", StringComparison.OrdinalIgnoreCase);
                }
                catch { return false; }
            });
        }

        protected static void WaitForHeaderReady(IWebDriver driver, TimeSpan timeout)
        {
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, timeout);
            wait.Until(d => d.FindElements(By.CssSelector("a[aria-label='Cart']")).Count > 0);
        }
    }
}
