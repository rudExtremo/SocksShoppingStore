using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SocksShoppingStore.Data;
using SocksShoppingStore.Models;
using SocksShoppingStore.Services;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SocksShoppingStore.Controllers
{
    [Route("api/products")]
    [ApiController]
    [EnableRateLimiting("api")]
    public class ProductsApiController : ControllerBase
    {
        private static readonly JsonSerializerOptions JsonOpts = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        private static readonly DateTimeOffset CatalogLastModified = DateTimeOffset.UtcNow;

        [HttpGet]
        public IActionResult GetAllProducts(
            string? q,
            string? sort,
            decimal? minPrice,
            decimal? maxPrice,
            int page = 1,
            int pageSize = 10,
            string? culture = null)
        {
            // Load and optionally localize
            var items = ProductRepository.GetAllSocks();
            var cultureKey = NormalizeCulture(culture ?? CultureInfo.CurrentUICulture.Name);
            items = ProductCatalogLocalizer.Localize(items, cultureKey);

            // Filters
            if (!string.IsNullOrWhiteSpace(q))
            {
                var query = q.Trim();
                items = items
                    .Where(s => (s.Name?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)
                             || (s.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
            }
            if (minPrice.HasValue) items = items.Where(s => s.Price >= minPrice.Value).ToList();
            if (maxPrice.HasValue) items = items.Where(s => s.Price <= maxPrice.Value).ToList();

            // Sorting
            sort = string.IsNullOrWhiteSpace(sort) ? "name_asc" : sort;
            items = sort switch
            {
                "name_desc" => items.OrderByDescending(s => s.Name).ToList(),
                "price_asc" => items.OrderBy(s => s.Price).ThenBy(s => s.Name).ToList(),
                "price_desc" => items.OrderByDescending(s => s.Price).ThenBy(s => s.Name).ToList(),
                _ => items.OrderBy(s => s.Name).ToList()
            };

            // Paging
            page = page < 1 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            var total = items.Count;
            var pageItems = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Prepare payload and caching headers
            var payload = pageItems;
            var json = JsonSerializer.Serialize(payload, JsonOpts);
            var etag = ComputeETag(json);

            // Conditional requests: If-None-Match / If-Modified-Since
            var inm = Request.Headers["If-None-Match"].ToString();
            if (!string.IsNullOrEmpty(inm) && string.Equals(inm, etag, StringComparison.Ordinal))
            {
                Response.Headers["ETag"] = etag;
                Response.Headers["Last-Modified"] = CatalogLastModified.ToString("R");
                Response.Headers["X-Total-Count"] = total.ToString(CultureInfo.InvariantCulture);
                return StatusCode(StatusCodes.Status304NotModified);
            }

            if (DateTimeOffset.TryParse(Request.Headers["If-Modified-Since"], out var ims))
            {
                if (CatalogLastModified <= ims)
                {
                    Response.Headers["ETag"] = etag;
                    Response.Headers["Last-Modified"] = CatalogLastModified.ToString("R");
                    Response.Headers["X-Total-Count"] = total.ToString(CultureInfo.InvariantCulture);
                    return StatusCode(StatusCodes.Status304NotModified);
                }
            }

            Response.Headers["ETag"] = etag;
            Response.Headers["Last-Modified"] = CatalogLastModified.ToString("R");
            Response.Headers["Cache-Control"] = "public, max-age=60";
            Response.Headers["X-Total-Count"] = total.ToString(CultureInfo.InvariantCulture);
            return Content(json, "application/json", Encoding.UTF8);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetProduct(int id, string? culture = null)
        {
            var sock = ProductRepository.GetSockById(id);
            if (sock == null) return NotFound();

            var cultureKey = NormalizeCulture(culture ?? CultureInfo.CurrentUICulture.Name);
            sock = ProductCatalogLocalizer.Localize(sock, cultureKey);

            var json = JsonSerializer.Serialize(sock, JsonOpts);
            var etag = ComputeETag(json);

            var inm = Request.Headers["If-None-Match"].ToString();
            if (!string.IsNullOrEmpty(inm) && string.Equals(inm, etag, StringComparison.Ordinal))
            {
                Response.Headers["ETag"] = etag;
                Response.Headers["Last-Modified"] = CatalogLastModified.ToString("R");
                return StatusCode(StatusCodes.Status304NotModified);
            }

            if (DateTimeOffset.TryParse(Request.Headers["If-Modified-Since"], out var ims))
            {
                if (CatalogLastModified <= ims)
                {
                    Response.Headers["ETag"] = etag;
                    Response.Headers["Last-Modified"] = CatalogLastModified.ToString("R");
                    return StatusCode(StatusCodes.Status304NotModified);
                }
            }

            Response.Headers["ETag"] = etag;
            Response.Headers["Last-Modified"] = CatalogLastModified.ToString("R");
            Response.Headers["Cache-Control"] = "public, max-age=60";
            return Content(json, "application/json", Encoding.UTF8);
        }

        private static string NormalizeCulture(string? culture)
        {
            if (string.IsNullOrWhiteSpace(culture)) return "en";
            return culture.StartsWith("ru", StringComparison.OrdinalIgnoreCase) ? "ru" : "en";
        }

        private static string ComputeETag(string json)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(json);
            var hash = sha.ComputeHash(bytes);
            return "\"" + Convert.ToHexString(hash) + "\""; // quoted ETag
        }
    }
}
