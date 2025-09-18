using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore;
using SocksShoppingStore.Controllers;
using SocksShoppingStore.Models;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("Unit")]
    
    [AllureFeature("Models/Options")]
    [AllureLabel("package", "SocksShoppingStore.Tests.Unit")]
    [AllureLabel("area", "Unit")]
    [AllureLabel("type", "Functional")]
    [AllureLabel("flow", "Positive")]
    [Category("Unit")]
    [Category("Positive")]
    public class ModelsAndOptionsTests
    {
        [Test]
        [AllureDescription(@"What: Verify ShowRequestId reflects whether RequestId is set.
Steps:
1) Create ErrorViewModel with null and non-null RequestId.
Expected: null => false; non-null => true.")]
        public void ErrorViewModel_ShowRequestId_ReflectsRequestId()
        {
            var m1 = new ErrorViewModel { RequestId = null };
            Assert.That(m1.ShowRequestId, Is.False);
            var m2 = new ErrorViewModel { RequestId = "abc" };
            Assert.That(m2.ShowRequestId, Is.True);
        }

        [Test]
        [AllureDescription(@"What: Verify default RateOptions values are positive.
Steps:
1) Instantiate RateOptions.
Expected: GlobalPerMinute>0; ApiPerMinute>0.")]
        public void RateOptions_Defaults_ArePositive()
        {
            var o = new RateOptions();
            Assert.That(o.GlobalPerMinute, Is.GreaterThan(0));
            Assert.That(o.ApiPerMinute, Is.GreaterThan(0));
        }

        [Test]
        [AllureDescription(@"What: Verify default LegalOptions fields are not empty.
Steps:
1) Instantiate LegalOptions.
Expected: ControllerName and ContactEmail are not empty.")]
        public void LegalOptions_Defaults_AreNotEmpty()
        {
            var o = new LegalOptions();
            Assert.That(o.ControllerName, Is.Not.Empty);
            Assert.That(o.ContactEmail, Is.Not.Empty);
        }
    }
}

