using NUnit.Framework;
using SocksShoppingStore;
using SocksShoppingStore.Helpers;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class RequestMetricsAndHelpersTests
    {
        [Test]
        public void RequestMetrics_Computes_Percentiles()
        {
            var m = new RequestMetrics(10);
            var arr = new double[] { 1,2,3,4,5,6,7,8,9,10 };
            for (int i = 0; i < arr.Length; i++) m.Record(200, arr[i]);
            dynamic snap = m.Snapshot();
            Assert.That((long)snap.counts.ok, Is.GreaterThan(0));
            Assert.That((double)snap.latency_ms.p50, Is.GreaterThan(0));
            Assert.That((int)snap.latency_ms.samples, Is.EqualTo(10));
        }

        [Test]
        public void Currency_Eur_Returns_EuroSymbol()
        {
            var s = Currency.Eur(12.34m);
            Assert.That(s.Contains("€") || s.Contains("\u20AC") || s.Contains("EUR", System.StringComparison.OrdinalIgnoreCase));
        }
    }
}

