using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Models;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("Unit")]
    
    [AllureFeature("Order")] 
    [AllureLabel("package", "SocksShoppingStore.Tests.Unit")]
    [AllureLabel("area", "Unit")]
    [AllureLabel("type", "Functional")]
    [AllureLabel("flow", "Positive")]
    [Category("Unit")]
    [Category("Positive")]
    public class OrderTests
    {
        [Test]
        public void Total_Sums_OrderItem_Subtotals()
        {
            var order = new Order
            {
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Name = "A", UnitPrice = 2m, Quantity = 3 }, // 6
                    new OrderItem { ProductId = 2, Name = "B", UnitPrice = 1.5m, Quantity = 2 } // 3
                }
            };
            Assert.That(order.Total, Is.EqualTo(9m));
        }
    }
}

