using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Data;
using SocksShoppingStore.Models;
using Allure.Net.Commons;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("Store")]
    [AllureSuite("Integration Tests")]
    [AllureFeature("Product Catalog")]
    [AllureLabel("package", "SocksShoppingStore.Tests.Integration")]
    [AllureLabel("area", "Integration")]
    [AllureLabel("type", "Functional")]
    [AllureLabel("flow", "Positive")]
    [AllureSeverity(SeverityLevel.normal)]
    [Category("Integration")]
    [Category("Unit")]
    [Category("Positive")]
    public class JsonProductRepositoryTests
    {
        [Test]
        [AllureDescription(@"What: Validate JSON repository CRUD flow.
Steps:
1) Create repo with temp path (seed copies legacy items).
2) Add new sock; update its description; delete it.
Expected: Add/Update/Delete succeed; item appears/changes/disappears accordingly.")]
        public void JsonRepository_CreateAddUpdateDelete_Works()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "socks-json-repo-tests");
            Directory.CreateDirectory(tempDir);
            var path = Path.Combine(tempDir, Guid.NewGuid().ToString("n") + ".json");

            var repo = new JsonProductRepository(path);

            // seed should copy legacy items
            var all = repo.GetAllSocks();
            Assert.That(all.Count, Is.GreaterThan(0));

            var s = new Sock { Name = "Test Sock", Description = "D", Price = 1.23m, ImageUrl = "/images/x.png" };
            repo.Add(s);
            var added = repo.GetAllSocks().FirstOrDefault(x => x.Name == "Test Sock");
            Assert.That(added, Is.Not.Null);

            added!.Description = "Updated";
            Assert.That(repo.Update(added), Is.True);
            var updated = repo.GetSockById(added.Id);
            Assert.That(updated!.Description, Is.EqualTo("Updated"));

            Assert.That(repo.Delete(added.Id), Is.True);
            Assert.That(repo.GetSockById(added.Id), Is.Null);
        }
    }
}
