using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Data;
using SocksShoppingStore.Models;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("Unit")]
    [Category("Unit")]
    [Category("Positive")]
    public class JsonProductRepositoryUnitTests
    {
        [Test]
        [AllureDescription(@"What: Validate JSON repository CRUD flow (unit style).
Steps:
1) Create repo with temp path (seed from legacy).
2) Add new sock; update description; delete.
Expected: Operations succeed accordingly.")]
        public void JsonRepository_CRUD_Works()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "socks-json-repo-unit-tests");
            Directory.CreateDirectory(tempDir);
            var path = Path.Combine(tempDir, Guid.NewGuid().ToString("n") + ".json");

            var repo = new JsonProductRepository(path);
            var all = repo.GetAllSocks();
            Assert.That(all.Count, Is.GreaterThan(0));

            var s = new Sock { Name = "Unit Sock", Description = "D", Price = 1.11m, ImageUrl = "/images/x.png" };
            repo.Add(s);
            var added = repo.GetAllSocks().FirstOrDefault(x => x.Name == "Unit Sock");
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

