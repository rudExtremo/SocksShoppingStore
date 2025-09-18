using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Controllers;
using SocksShoppingStore.Helpers;
using SocksShoppingStore.Models;
using SocksShoppingStore.Tests.TestDoubles;
using System.Text.Json;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("Unit")]
    [Category("Unit")]
    [Category("Positive")]
    public class CartControllerJsonUnitTests
    {
        private static (CartController ctrl, DefaultHttpContext ctx) Create()
        {
            var ctrl = new CartController();
            var ctx = new DefaultHttpContext { Session = new TestSession() };
            ctrl.ControllerContext = new ControllerContext { HttpContext = ctx };
            return (ctrl, ctx);
        }

        private static string Serialize(object? value) => JsonSerializer.Serialize(value, new JsonSerializerOptions{ WriteIndented=false });

        [Test]
        [AllureDescription(@"What: Verify AddToCart returns JSON when Accept=application/json and updates totals.
Steps:
1) Prepare empty session cart and set Accept header to application/json.
2) Call AddToCart(id=1), then call again to increment quantity.
Expected: JSON contains totalItems=2, uniqueItems=1, totalSum>0, and item with matching id and quantity.")]
        public void Cart_AddToCart_ReturnsJsonAndUpdatesTotals()
        {
            var (c, ctx) = Create();
            ctx.Request.Headers["Accept"] = "application/json";

            var r1 = c.AddToCart(1) as JsonResult;
            Assert.That(r1, Is.Not.Null);

            var r2 = c.AddToCart(1) as JsonResult;
            Assert.That(r2, Is.Not.Null);
            var json = Serialize(r2!.Value);
            StringAssert.Contains("\"totalItems\":2", json);
            StringAssert.Contains("\"uniqueItems\":1", json);
            StringAssert.Contains("\"item\":{", json);
            StringAssert.Contains("\"id\":1", json);
            StringAssert.Contains("\"quantity\":2", json);
        }

        [Test]
        [AllureDescription(@"What: Verify RemoveFromCart returns JSON payload and does not reduce below 1 when quantity==1.
Steps:
1) Add same item twice; remove once via RemoveFromCart.
Expected: Remaining quantity is 1 in JSON; totals reflect change.")]
        public void Cart_RemoveFromCart_ReturnsJson_AndStopsAtOne()
        {
            var (c, ctx) = Create();
            ctx.Request.Headers["Accept"] = "application/json";
            c.AddToCart(1);
            c.AddToCart(1);

            var r = c.RemoveFromCart(1) as JsonResult;
            Assert.That(r, Is.Not.Null);
            var json = Serialize(r!.Value);
            StringAssert.Contains("\"quantity\":1", json);
        }

        [Test]
        [AllureDescription(@"What: Verify SetQuantity returns JSON and removes item when quantity=0.
Steps:
1) Add item; SetQuantity(id, 0) with Accept=application/json.
Expected: JSON item has quantity=0 and subtotal=0.")]
        public void Cart_SetQuantity_Zero_RemovesAndReturnsJson()
        {
            var (c, ctx) = Create();
            ctx.Request.Headers["Accept"] = "application/json";
            c.AddToCart(1);

            var r = c.SetQuantity(1, 0) as JsonResult;
            Assert.That(r, Is.Not.Null);
            var json = Serialize(r!.Value);
            StringAssert.Contains("\"quantity\":0", json);
        }

        [Test]
        [AllureDescription(@"What: Verify DeleteItem and Clear return JSON payloads with zeroed totals.
Steps:
1) Add item; call DeleteItem and Clear with Accept=application/json.
Expected: JSON shows totals equal to zero.")]
        public void Cart_DeleteItem_And_Clear_ReturnJsonWithZeros()
        {
            var (c, ctx) = Create();
            ctx.Request.Headers["Accept"] = "application/json";
            c.AddToCart(1);

            var r1 = c.DeleteItem(1) as JsonResult;
            Assert.That(r1, Is.Not.Null);
            StringAssert.Contains("\"totalItems\":0", Serialize(r1!.Value));

            var r2 = c.Clear() as JsonResult;
            Assert.That(r2, Is.Not.Null);
            StringAssert.Contains("\"totalItems\":0", Serialize(r2!.Value));
        }
    }
}

