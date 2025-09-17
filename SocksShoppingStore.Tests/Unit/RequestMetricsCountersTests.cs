using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("Unit")]
    [Category("Positive")]
    public class RequestMetricsCountersTests
    {
        [Test]
        [AllureDescription(@"What: Verify counters increment for 2xx, 429 and 503 statuses.
Steps:
1) Record multiple samples with different status codes.
2) Snapshot counts.
Expected: ok>0; too_many>0; unavailable>0.")]
        public void RequestMetrics_Increments_Counters_ByStatus()
        {
            var m = new SocksShoppingStore.RequestMetrics(20);
            m.Record(200, 1);
            m.Record(201, 2);
            m.Record(429, 3);
            m.Record(503, 4);
            var json = System.Text.Json.JsonSerializer.Serialize(m.Snapshot());
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.That(root.GetProperty("counts").GetProperty("ok").GetInt64(), Is.GreaterThan(0));
            Assert.That(root.GetProperty("counts").GetProperty("too_many").GetInt64(), Is.GreaterThan(0));
            Assert.That(root.GetProperty("counts").GetProperty("unavailable").GetInt64(), Is.GreaterThan(0));
        }
    }
}
