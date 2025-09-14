using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Data;
using Allure.Net.Commons;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("Store")]
    [AllureSuite("Integration Tests")]
    [AllureFeature("Product Catalog")]
    [AllureLabel("package", "SocksShoppingStore.Tests.Integration")]
    [AllureLabel("area", "Integration")]
    [AllureLabel("type", "Functional")]
    [AllureLabel("flow", "Positive")]
    [AllureSeverity(SeverityLevel.normal)]
    [Category("Integration")]
    [Category("Unit")]
    [Category("Positive")]
    public class ProductRepositoryTests
    {
        [Test]
        public void GetAllSocks_Returns_Expected_Count()
        {
            var all = ProductRepository.GetAllSocks();
            Assert.That(all.Count, Is.GreaterThanOrEqualTo(8));
        }

        [Test]
        public void GetSockById_Returns_Null_For_Unknown()
        {
            var s = ProductRepository.GetSockById(-123);
            Assert.That(s, Is.Null);
        }
    }
}
