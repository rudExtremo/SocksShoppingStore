using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Models;
using SocksShoppingStore.Services;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("Unit")]
    
    [AllureFeature("Product Catalog")]
    [AllureLabel("package", "SocksShoppingStore.Tests.Unit")]
    [AllureLabel("area", "Unit")]
    [AllureLabel("type", "Functional")]
    [AllureLabel("flow", "Positive")]
    [Category("Unit")]
    [Category("Positive")]
    public class ProductCatalogLocalizerTests
    {
        [Test]
        [AllureDescription(@"What: Verify RU localization overrides product fields as expected.
Steps:
1) Build a list with one English item.
2) Localize with 'ru'.
Expected: Name or description differs from English.")]
        public void Localizer_List_Russian_Overrides_NameOrDescription()
        {
            var items = new List<Sock>
            {
                new Sock { Id = 1, Name = "Coder's Comfort", Description = "Breathable cotton." }
            };

            var ru = ProductCatalogLocalizer.Localize(items, "ru");
            Assert.That(ru[0].Name, Is.Not.EqualTo("Coder's Comfort"));
        }

        [Test]
        [AllureDescription(@"What: Verify EN localization leaves product unchanged.
Steps:
1) Create English sock.
2) Localize with 'en'.
Expected: Name equals original English name.")]
        public void Localizer_Single_English_Default_NoChange()
        {
            var s = new Sock { Id = 1, Name = "Coder's Comfort", Description = "Breathable cotton." };
            var en = ProductCatalogLocalizer.Localize(s, "en");
            Assert.That(en.Name, Is.EqualTo("Coder's Comfort"));
        }
    }
}

