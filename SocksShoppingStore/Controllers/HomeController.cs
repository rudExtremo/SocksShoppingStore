using Microsoft.AspNetCore.Mvc;
using SocksShoppingStore.Models;
using SocksShoppingStore.Data; // Работа с репозиторием продуктов

namespace SocksShoppingStore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string? q)
        {
            // Получаем все товары из репозитория
            var socks = ProductRepository.GetAllSocks();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var query = q.Trim();
                socks = socks
                    .Where(s => (s.Name?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)
                             || (s.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
                ViewData["Query"] = q;
            }

            return View(socks);
        }

        // Статическая страница Privacy
        public IActionResult Privacy()
        {
            return View();
        }
    }
}

