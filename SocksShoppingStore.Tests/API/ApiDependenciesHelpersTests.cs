using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Helpers;
using SocksShoppingStore.Models;
using Microsoft.AspNetCore.Http;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("API-Smoke")]
    [Category("Positive")]
    public class ApiDependenciesHelpersTests
    {
        [Test]
        [AllureDescription(@"What: Currency.Eur produces a string with euro symbol or code (API dependency).
Steps:
1) Format 12.34m.
Expected: Result contains '€' or 'EUR'.")]
        public void Currency_Eur_Formats()
        {
            var s = Currency.Eur(12.34m);
            Assert.That(s.Contains("€") || s.Contains("\u20AC") || s.Contains("EUR", System.StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        [AllureDescription(@"What: SessionHelper serializes and deserializes ShoppingCart (API dependency).
Steps:
1) Create cart with 2 items; Set/Get via session.
Expected: Deserialized not null and item count equals 2.")]
        public void SessionHelper_Serializes_ShoppingCart()
        {
            ISession session = new SocksShoppingStore.Tests.TestDoubles.TestSession();
            var cart = new ShoppingCart();
            cart.AddItem(new Sock{ Id=1, Name="A", Price=1.0m });
            cart.AddItem(new Sock{ Id=2, Name="B", Price=2.0m });
            session.Set("Cart", cart);
            var loaded = session.Get<ShoppingCart>("Cart");
            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded!.GetTotalItems(), Is.EqualTo(2));
        }

        [Test]
        [AllureDescription(@"What: ErrorViewModel.ShowRequestId reflects RequestId (API dependency).
Steps:
1) Set null and non-null request id.
Expected: ShowRequestId false then true.")]
        public void ErrorViewModel_ShowRequestId_Works()
        {
            var m1 = new ErrorViewModel { RequestId = null };
            Assert.That(m1.ShowRequestId, Is.False);
            var m2 = new ErrorViewModel { RequestId = "abc" };
            Assert.That(m2.ShowRequestId, Is.True);
        }
    }
}

