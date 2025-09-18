using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Data;
using Allure.Net.Commons;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("Integration")]
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
        [AllureDescription(@"What: Verify legacy in-memory repository returns non-empty list.
Steps:
1) Call ProductRepository.GetAllSocks().
Expected: Count >= 8.")]
        public void Repository_GetAllSocks_Returns_ExpectedCount()
        {
            var all = ProductRepository.GetAllSocks();
            Assert.That(all.Count, Is.GreaterThanOrEqualTo(8));
        }

        [Test]
        [AllureDescription(@"What: Verify GetSockById returns null for unknown id.
Steps:
1) Call GetSockById with negative id.
Expected: Null.")]
        public void Repository_GetSockById_ReturnsNull_ForUnknown()
        {
            var s = ProductRepository.GetSockById(-123);
            Assert.That(s, Is.Null);
        }
    }
}
