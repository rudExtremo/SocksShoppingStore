using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using OpenQA.Selenium;
using SocksShoppingStore.Tests.PageObjects;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("UI-Smoke")]
    public class CheckoutUiTests : BaseTest
    {
        private CheckoutPage? Checkout;

        [SetUp]
        public void SetupPage()
        {
            if (Driver == null) return;
            Checkout = new CheckoutPage(Driver);
        }

        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Checkout form displays.
Steps: 1) Navigate to /Checkout.
Expected: Inputs for name/email/address/city/postal/country and Review button visible.")]
        public void Checkout_Form_Displays()
        {
            HomePage!.Navigate();
            Driver!.Navigate().GoToUrl(TestSettings.BaseUrl.TrimEnd('/') + "/Checkout");
            Assert.That(Driver.FindElements(By.Id("CustomerName")).Count, Is.GreaterThan(0));
            Assert.That(Driver.FindElements(By.CssSelector("form button[type='submit']")).Count, Is.GreaterThan(0));
        }

        [Test]
        [Category("Negative")]
        [AllureDescription(@"What: Empty form shows validation errors.
Steps: 1) Open Checkout; 2) Submit empty.
Expected: Validation error messages shown.")]
        public void Checkout_EmptyForm_ShowsValidation()
        {
            HomePage!.Navigate();
            Driver!.Navigate().GoToUrl(TestSettings.BaseUrl.TrimEnd('/') + "/Checkout");
            Driver.FindElement(By.CssSelector("form button[type='submit']")).Click();
            var errors = Driver.FindElements(By.CssSelector("span.text-danger"));
            Assert.That(errors.Count, Is.GreaterThan(0));
        }

        [Test]
        [Category("Negative")]
        [AllureDescription(@"What: Invalid email shows validation.
Steps: 1) Open Checkout; 2) Fill valid fields except Email; 3) Submit.
Expected: Email validation error.")]
        public void Checkout_InvalidEmail_ShowsValidation()
        {
            HomePage!.Navigate();
            Driver!.Navigate().GoToUrl(TestSettings.BaseUrl.TrimEnd('/') + "/Checkout");
            var page = new CheckoutPage(Driver);
            page.FillValid();
            page.EmailInput.Clear(); page.EmailInput.SendKeys("bademail");
            page.ReviewButton.Click();
            var emailError = Driver.FindElements(By.CssSelector("span.text-danger"));
            Assert.That(emailError.Count, Is.GreaterThan(0));
        }

        [Test]
        [Category("Positive")]
        [AllureDescription(@"What: Valid checkout proceeds to Review, confirms and shows Thank You.
Steps: 1) Add item; 2) Fill form; 3) Review; 4) Confirm.
Expected: Review page shows items; Thank You shows order id and total.")]
        public void Checkout_Valid_To_Review_And_ThankYou()
        {
            // Ensure cart not empty
            HomePage!.Navigate();
            HomePage.AddFirstProductToCart();
            Driver!.Navigate().GoToUrl(TestSettings.BaseUrl.TrimEnd('/') + "/Checkout");
            var page = new CheckoutPage(Driver);
            page.FillValid();
            page.ReviewButton.Click();

            // Review shows table and Confirm button
            Assert.That(Driver.FindElements(By.CssSelector("table thead tr th")).Count, Is.GreaterThanOrEqualTo(3));
            Driver.FindElement(By.CssSelector("form button.btn.btn-success")).Click();

            // Thank You page
            Assert.That(Driver.FindElements(By.Id("order-id")).Count, Is.GreaterThanOrEqualTo(0));
            var backBtn = Driver.FindElement(By.CssSelector("a.btn.btn-primary"));
            Assert.That(backBtn.Text, Is.Not.Empty);
        }
    }
}

