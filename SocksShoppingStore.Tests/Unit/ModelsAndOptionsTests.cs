using NUnit.Framework;
using Allure.NUnit;
using SocksShoppingStore;
using SocksShoppingStore.Controllers;
using SocksShoppingStore.Models;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("Unit")]
    [Category("Positive")]
    public class ModelsAndOptionsTests
    {
        [Test]
        public void ErrorViewModel_ShowRequestId_Reflects_RequestId()
        {
            var m1 = new ErrorViewModel { RequestId = null };
            Assert.That(m1.ShowRequestId, Is.False);
            var m2 = new ErrorViewModel { RequestId = "abc" };
            Assert.That(m2.ShowRequestId, Is.True);
        }

        [Test]
        public void RateOptions_Defaults()
        {
            var o = new RateOptions();
            Assert.That(o.GlobalPerMinute, Is.GreaterThan(0));
            Assert.That(o.ApiPerMinute, Is.GreaterThan(0));
        }

        [Test]
        public void LegalOptions_Defaults()
        {
            var o = new LegalOptions();
            Assert.That(o.ControllerName, Is.Not.Empty);
            Assert.That(o.ContactEmail, Is.Not.Empty);
        }
    }
}
