using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Models;
using SocksShoppingStore.Services;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("Магазин")]
    [AllureSuite("Юнит-тесты")]
    [AllureFeature("Каталог товаров")]
    [AllureLabel("package", "SocksShoppingStore.Tests.Unit")]
    [Category("Unit")]
    [Category("Positive")]
    public class ProductCatalogLocalizerTests
    {
        [Test]
        public void Localize_List_Russian_Overrides_Name_Or_Description()
        {
            var items = new List<Sock>
            {
                new Sock { Id = 1, Name = "Coder's Comfort", Description = "Breathable cotton." }
            };

            var ru = ProductCatalogLocalizer.Localize(items, "ru");
            Assert.That(ru[0].Name, Is.Not.EqualTo("Coder's Comfort"));
        }

        [Test]
        public void Localize_Single_English_Default_NoChange()
        {
            var s = new Sock { Id = 1, Name = "Coder's Comfort", Description = "Breathable cotton." };
            var en = ProductCatalogLocalizer.Localize(s, "en");
            Assert.That(en.Name, Is.EqualTo("Coder's Comfort"));
        }
    }
}
