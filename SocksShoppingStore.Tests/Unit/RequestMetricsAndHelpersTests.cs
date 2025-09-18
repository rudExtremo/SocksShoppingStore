using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore;
using SocksShoppingStore.Helpers;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("Unit")]
    [AllureEpic("Store")]
    [AllureSuite("Unit Tests")]
    [AllureFeature("Metrics")]
    [AllureLabel("package", "SocksShoppingStore.Tests.Unit")]
    [AllureLabel("area", "Unit")]
    [AllureLabel("type", "Functional")]
    [AllureLabel("flow", "Positive")]
    [Category("Unit")]
    [Category("Positive")]
    public class RequestMetricsAndHelpersTests
    {
        [Test]
        [AllureDescription(@"What: Verify RequestMetrics computes percentiles and sample count.
Steps:
1) Record 10 samples.
2) Read snapshot and serialize to JSON.
Expected: ok>0; p50>0; samples==10.")]
        public void RequestMetrics_Computes_Percentiles_AndSamples()
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
        [AllureDescription(@"What: Verify Currency.Eur formats string with euro symbol or 'EUR'.
Steps:
1) Format 12.34m.
Expected: Output contains '€' or 'EUR'.")]
        public void Currency_Eur_Returns_EuroSymbolOrCode()
        {
            var s = Currency.Eur(12.34m);
            Assert.That(s.Contains("€") || s.Contains("\u20AC") || s.Contains("EUR", System.StringComparison.OrdinalIgnoreCase));
        }
    }
}

