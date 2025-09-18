using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SocksShoppingStore.Data;
using SocksShoppingStore.Models;
using SocksShoppingStore.Services;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("Integration")]
    [Category("API-Smoke")]
    [Category("Positive")]
    public class ApiDependenciesRepoAndServiceTests
    {
        [Test]
        [AllureDescription(@"What: Legacy repository basic behaviors (API dependency).
Steps:
1) GetAllSocks -> count>=8.
2) GetSockById(1) not null; (-1) null.")]
        public void LegacyRepository_Basics()
        {
            var repo = new LegacyProductRepository();
            var all = repo.GetAllSocks();
            Assert.That(all.Count, Is.GreaterThanOrEqualTo(8));
            Assert.That(repo.GetSockById(1), Is.Not.Null);
            Assert.That(repo.GetSockById(-1), Is.Null);
        }

        [Test]
        [AllureDescription(@"What: Legacy repository CRUD via adapter (API dependency).
Steps:
1) Add new sock; Update; Delete.
Expected: Update true; Delete true; then null by id.")]
        public void LegacyRepository_CRUD()
        {
            var repo = new LegacyProductRepository();
            var s = new Sock { Id = 0, Name = "Legacy API", Description = "D", Price = 2.0m, ImageUrl = "/img.png" };
            repo.Add(s);
            var added = repo.GetAllSocks().FirstOrDefault(x => x.Name == "Legacy API");
            Assert.That(added, Is.Not.Null);
            added!.Description = "U";
            Assert.That(repo.Update(added), Is.True);
            Assert.That(repo.Delete(added.Id), Is.True);
            Assert.That(repo.GetSockById(added.Id), Is.Null);
        }

        [Test]
        [AllureDescription(@"What: Json repository CRUD (API dependency).
Steps:
1) Add/Update/Delete item.
Expected: Operations succeed.")]
        public void JsonRepository_CRUD()
        {
            var baseDir = Path.Combine(Path.GetTempPath(), "socks-json-api-tests");
            Directory.CreateDirectory(baseDir);
            var path = Path.Combine(baseDir, Guid.NewGuid().ToString("n") + ".json");
            var repo = new JsonProductRepository(path);
            var s = new Sock { Name = "API Sock", Description = "D", Price = 1.0m, ImageUrl = "/img.png" };
            repo.Add(s);
            var added = repo.GetAllSocks().FirstOrDefault(x => x.Name == "API Sock");
            Assert.That(added, Is.Not.Null);
            added!.Description = "U";
            Assert.That(repo.Update(added), Is.True);
            Assert.That(repo.Delete(added.Id), Is.True);
        }

        [Test]
        [AllureDescription(@"What: Catalog localizer EN/RU (API dependency).
Steps:
1) Localize list with 'ru' and 'en'.
Expected: RU modifies name; EN leaves it.")]
        public void ProductCatalogLocalizer_RU_EN()
        {
            var items = new List<Sock> { new Sock { Id = 1, Name = "Coder's Comfort", Description = "Breathable." } };
            var ru = ProductCatalogLocalizer.Localize(items.Select(s=>new Sock{ Id=s.Id, Name=s.Name, Description=s.Description, Price=0}).ToList(), "ru");
            Assert.That(ru[0].Name, Is.Not.EqualTo("Coder's Comfort"));
            var en = ProductCatalogLocalizer.Localize(items.Select(s=>new Sock{ Id=s.Id, Name=s.Name, Description=s.Description, Price=0}).ToList(), "en");
            Assert.That(en[0].Name, Is.EqualTo("Coder's Comfort"));
        }
    }
}
