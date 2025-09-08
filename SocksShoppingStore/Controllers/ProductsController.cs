using Microsoft.AspNetCore.Mvc;
using SocksShoppingStore.Data;

namespace SocksShoppingStore.Controllers
{
    public class ProductsController : Controller
    {
        [HttpGet]
        public IActionResult Details(int id)
        {
            var sock = ProductRepository.GetSockById(id);
            if (sock == null) return NotFound();
            return View(sock);
        }
    }
}

