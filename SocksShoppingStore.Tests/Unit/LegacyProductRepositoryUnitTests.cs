using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Data;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("Unit")]
    [Category("Unit")]
    [Category("Positive")]
    public class LegacyProductRepositoryUnitTests
    {
        [Test]
        [AllureDescription(@"What: Verify legacy repository returns list and resolves items by id.
Steps:
1) GetAllSocks() -> count >= 8.
2) GetSockById(1) -> not null; GetSockById(-1) -> null.")]
        public void LegacyRepo_GetAll_And_ById()
        {
            var repo = new LegacyProductRepository();
            var all = repo.GetAllSocks();
            Assert.That(all.Count, Is.GreaterThanOrEqualTo(8));
            Assert.That(repo.GetSockById(1), Is.Not.Null);
            Assert.That(repo.GetSockById(-1), Is.Null);
        }
    }
}
