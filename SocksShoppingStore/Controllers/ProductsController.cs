using Microsoft.AspNetCore.Mvc;
using SocksShoppingStore.Data;
using SocksShoppingStore.Services;
using System.Globalization;

namespace SocksShoppingStore.Controllers
{
    public class ProductsController : Controller
    {
        [HttpGet]
        public IActionResult Details(int id)
        {
            var sock = ProductRepository.GetSockById(id);
            if (sock == null) return NotFound();
            sock = ProductCatalogLocalizer.Localize(sock, CultureInfo.CurrentUICulture.Name);
            return View(sock);
        }
    }
}
