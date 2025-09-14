using NUnit.Framework;
using Allure.NUnit;
using SocksShoppingStore;
using SocksShoppingStore.Helpers;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("Unit")]
    [Category("Positive")]
    public class RequestMetricsAndHelpersTests
    {
        [Test]
        public void RequestMetrics_Computes_Percentiles()
        {
            var m = new RequestMetrics(10);
            var arr = new double[] { 1,2,3,4,5,6,7,8,9,10 };
            for (int i = 0; i < arr.Length; i++) m.Record(200, arr[i]);
            var snap = m.Snapshot();
            var json = System.Text.Json.JsonSerializer.Serialize(snap);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.That(root.GetProperty("counts").GetProperty("ok").GetInt64(), Is.GreaterThan(0));
            Assert.That(root.GetProperty("latency_ms").GetProperty("p50").GetDouble(), Is.GreaterThan(0));
            Assert.That(root.GetProperty("latency_ms").GetProperty("samples").GetInt32(), Is.EqualTo(10));
        }

        [Test]
        public void Currency_Eur_Returns_EuroSymbol()
        {
            var s = Currency.Eur(12.34m);
            Assert.That(s.Contains("€") || s.Contains("\u20AC") || s.Contains("EUR", System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
