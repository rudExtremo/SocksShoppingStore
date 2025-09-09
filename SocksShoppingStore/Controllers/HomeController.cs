using Microsoft.AspNetCore.Mvc;
using SocksShoppingStore.Models;
using SocksShoppingStore.Data; // Repository access
using System.Globalization;
using SocksShoppingStore.Services;

namespace SocksShoppingStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly SocksShoppingStore.Data.IProductRepository _repo;
        public HomeController(SocksShoppingStore.Data.IProductRepository? repo = null)
        {
            _repo = repo ?? new SocksShoppingStore.Data.LegacyProductRepository();
        }
        public IActionResult Index(
            string? q,
            string? sort,
            decimal? minPrice,
            decimal? maxPrice,
            int page = 1,
            int pageSize = 6)
        {
            // Load catalog
            var socks = _repo.GetAllSocks();

            // Localize product names/descriptions based on current UI culture
            var culture = CultureInfo.CurrentUICulture.Name;
            socks = ProductCatalogLocalizer.Localize(socks, culture);

            // Text search
            if (!string.IsNullOrWhiteSpace(q))
            {
                var query = q.Trim();
                socks = socks
                    .Where(s => (s.Name?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)
                             || (s.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
            }

            // Price filters
            if (minPrice.HasValue)
            {
                socks = socks.Where(s => s.Price >= minPrice.Value).ToList();
            }
            if (maxPrice.HasValue)
            {
                socks = socks.Where(s => s.Price <= maxPrice.Value).ToList();
            }

            // Sorting
            sort = string.IsNullOrWhiteSpace(sort) ? "name_asc" : sort;
            socks = sort switch
            {
                "name_desc" => socks.OrderByDescending(s => s.Name).ToList(),
                "price_asc" => socks.OrderBy(s => s.Price).ThenBy(s => s.Name).ToList(),
                "price_desc" => socks.OrderByDescending(s => s.Price).ThenBy(s => s.Name).ToList(),
                _ => socks.OrderBy(s => s.Name).ToList()
            };

            // Pagination
            page = page < 1 ? 1 : page;
            pageSize = pageSize <= 0 ? 6 : pageSize;
            var total = socks.Count;
            var items = socks.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var vm = new CatalogViewModel
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize,
                Query = q ?? string.Empty,
                Sort = sort,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            return View(vm);
        }

        // Privacy page
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
