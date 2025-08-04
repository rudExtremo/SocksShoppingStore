using Microsoft.AspNetCore.Mvc;
using SocksShoppingStore.Models;
using SocksShoppingStore.Data; // Подключаем наш новый репозиторий

namespace SocksShoppingStore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Получаем все товары из единого репозитория
            var socks = ProductRepository.GetAllSocks();
            return View(socks);
        }

        // Возвращаем метод для страницы Privacy
        public IActionResult Privacy()
        {
            return View();
        }
    }
}