using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore;
using SocksShoppingStore.Data;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [Category("API-Smoke")]
    [Category("Positive")]
    public class ApiDependenciesMiscTests
    {
        [Test]
        [AllureDescription(@"What: RequestMetrics snapshot fields present (API dependency).
Steps:
1) Record samples; read snapshot.
Expected: counts.ok>0; latency_ms.samples>0.")]
        public void RequestMetrics_Snapshot()
        {
            var m = new RequestMetrics(10);
            for (int i = 1; i <= 5; i++) m.Record(200, i);
            var json = System.Text.Json.JsonSerializer.Serialize(m.Snapshot());
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.That(root.GetProperty("counts").GetProperty("ok").GetInt64(), Is.GreaterThan(0));
            Assert.That(root.GetProperty("latency_ms").GetProperty("samples").GetInt32(), Is.GreaterThan(0));
        }

        [Test]
        [AllureDescription(@"What: RequestMetrics empty snapshot returns zero percentiles.
Steps:
1) Snapshot without records.
Expected: p50==0; samples==0.")]
        public void RequestMetrics_EmptySnapshot_Zeros()
        {
            var m = new RequestMetrics(5);
            var json = System.Text.Json.JsonSerializer.Serialize(m.Snapshot());
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.That(root.GetProperty("latency_ms").GetProperty("p50").GetDouble(), Is.EqualTo(0));
            Assert.That(root.GetProperty("latency_ms").GetProperty("samples").GetInt32(), Is.EqualTo(0));
        }

        [Test]
        [AllureDescription(@"What: Static ProductRepository returns items (API dependency).
Steps:
1) Call GetAllSocks; GetSockById(1).
Expected: Non-empty list; non-null item for id=1.")]
        public void ProductRepository_Static_Basics()
        {
            var all = ProductRepository.GetAllSocks();
            Assert.That(all.Count, Is.GreaterThan(0));
            Assert.That(ProductRepository.GetSockById(1), Is.Not.Null);
            Assert.That(ProductRepository.GetSockById(-1), Is.Null);
        }

        [Test]
        [AllureDescription(@"What: Json repository returns false on update/delete unknown (API dependency).
Steps:
1) Create repo; call Update/Delete with missing id.
Expected: Returns false.")]
        public void JsonRepository_UpdateDelete_ReturnsFalse_WhenUnknown()
        {
            var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.Guid.NewGuid().ToString("n") + ".json");
            var repo = new JsonProductRepository(path);
            Assert.That(repo.Update(new SocksShoppingStore.Models.Sock { Id = -1, Name = "X", Price = 1 }), Is.False);
            Assert.That(repo.Delete(-1), Is.False);
        }
    }
}
