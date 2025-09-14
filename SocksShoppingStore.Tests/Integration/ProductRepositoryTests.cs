using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Data;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("Магазин")]
    [AllureSuite("Интеграционные тесты")]
    [AllureFeature("Каталог товаров")]
    [AllureLabel("package", "SocksShoppingStore.Tests.Integration")]
    [Category("Integration")]
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

