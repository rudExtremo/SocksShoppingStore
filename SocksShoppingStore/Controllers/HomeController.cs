using Microsoft.AspNetCore.Mvc;
using SocksShoppingStore.Models;
using SocksShoppingStore.Data; // Работа с репозиторием продуктов
using System.Globalization;
using SocksShoppingStore.Services;

namespace SocksShoppingStore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string? q)
        {
            // Получаем все товары из репозитория
            var socks = ProductRepository.GetAllSocks();

            // Localize product names/descriptions based on current UI culture
            var culture = CultureInfo.CurrentUICulture.Name;
            socks = ProductCatalogLocalizer.Localize(socks, culture);

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
