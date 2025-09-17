using OpenQA.Selenium;

namespace SocksShoppingStore.Tests.PageObjects
{
    public class CheckoutPage
    {
        private readonly IWebDriver _driver;
        public CheckoutPage(IWebDriver driver) { _driver = driver; }

        public void Navigate(string baseUrl) => _driver.Navigate().GoToUrl(baseUrl.TrimEnd('/') + "/Checkout");

        public IWebElement NameInput => _driver.FindElement(By.Id("CustomerName"));
        public IWebElement EmailInput => _driver.FindElement(By.Id("Email"));
        public IWebElement Address1Input => _driver.FindElement(By.Id("AddressLine1"));
        public IWebElement Address2Input => _driver.FindElement(By.Id("AddressLine2"));
        public IWebElement CityInput => _driver.FindElement(By.Id("City"));
        public IWebElement PostalCodeInput => _driver.FindElement(By.Id("PostalCode"));
        public IWebElement CountryInput => _driver.FindElement(By.Id("Country"));
        public IWebElement ReviewButton => _driver.FindElement(By.CssSelector("form button[type='submit']"));
        public IWebElement BackToCartLink => _driver.FindElement(By.CssSelector("a.btn.btn-outline-secondary"));

        public void FillValid()
        {
            NameInput.Clear(); NameInput.SendKeys("John Doe");
            EmailInput.Clear(); EmailInput.SendKeys("john@example.com");
            Address1Input.Clear(); Address1Input.SendKeys("Street 1");
            CityInput.Clear(); CityInput.SendKeys("City");
            PostalCodeInput.Clear(); PostalCodeInput.SendKeys("12345");
            CountryInput.Clear(); CountryInput.SendKeys("US");
        }
    }
}

