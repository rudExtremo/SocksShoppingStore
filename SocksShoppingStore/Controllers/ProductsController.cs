using Microsoft.AspNetCore.Mvc;
using SocksShoppingStore.Data;
using SocksShoppingStore.Services;
using System.Globalization;

namespace SocksShoppingStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly SocksShoppingStore.Data.IProductRepository _repo;
        public ProductsController(SocksShoppingStore.Data.IProductRepository? repo = null)
        {
            _repo = repo ?? new SocksShoppingStore.Data.LegacyProductRepository();
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var sock = _repo.GetSockById(id);
            if (sock == null) return NotFound();
            sock = ProductCatalogLocalizer.Localize(sock, CultureInfo.CurrentUICulture.Name);
            return View(sock);
        }
    }
}
