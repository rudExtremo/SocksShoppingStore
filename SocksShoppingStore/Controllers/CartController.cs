// Файл: Controllers/CartController.cs
using Microsoft.AspNetCore.Mvc;

namespace SocksShoppingStore.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            // Мы ничего не делаем, страница сама возьмет данные из TempData
            return View();
        }
    }
}