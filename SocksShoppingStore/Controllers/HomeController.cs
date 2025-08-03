// Файл: Controllers/HomeController.cs
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SocksShoppingStore.Models;

namespace SocksShoppingStore.Controllers
{
    public class HomeController : Controller
    {
        // Создаем "ненастоящую" базу данных носков
        private static readonly List<Sock> _socks = new List<Sock>
        {
            new Sock { Id = 1, Name = "Носки 'Программист'", Description = "Для долгих ночей кодинга", Price = 350.00m, ImageUrl = "/images/coder_socks.png" },
            new Sock { Id = 2, Name = "Носки 'Менеджер'", Description = "Чтобы уверенно шагать по офису", Price = 450.50m, ImageUrl = "/images/manager_socks.png" },
            new Sock { Id = 3, Name = "Носки 'Тестировщик'", Description = "В них легко найти любой баг", Price = 399.99m, ImageUrl = "/images/qa_socks.png" },
            new Sock { Id = 4, Name = "Носки 'СЕО'", Description = "Бизнес начинается с носочков", Price = 999.99m, ImageUrl = "/images/CEO.png" },
            new Sock { Id = 5, Name = "Носки 'Аналитик'", Description = "По статистике в носках теплее", Price = 500.15m, ImageUrl = "/images/analitic.png" },
            new Sock { Id = 6, Name = "Носки 'Маркетолог'", Description = "Теплые ноги повышабт продажи", Price = 399.99m, ImageUrl = "/images/market.png" },
            new Sock { Id = 7, Name = "Носки 'Безопасник'", Description = "Носки без дырок - первый шаг к безопасности", Price = 200.00m, ImageUrl = "/images/security.png" },
            new Sock { Id = 8, Name = "Носки 'Дизайнер'", Description = "Для самых уникальных", Price = 234.33m, ImageUrl = "/images/designer.png" }
        };

        public IActionResult Index()
        {
            // Передаем список носков на страницу
            return View(_socks);
        }

        // Этот метод можешь пока не трогать или удалить
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult AddToCart(int id)
        {
            // Находим носок по Id в нашем списке
            var sock = _socks.FirstOrDefault(s => s.Id == id);
            if (sock != null)
            {
                // Временно сохраняем имя товара, чтобы показать его в корзине
                TempData["AddedProductName"] = sock.Name;
            }
            // Перенаправляем на страницу корзины
            return RedirectToAction("Index", "Cart");
        }
    }
}