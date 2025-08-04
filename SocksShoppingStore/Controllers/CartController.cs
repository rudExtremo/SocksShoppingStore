using Microsoft.AspNetCore.Mvc;
using SocksShoppingStore.Models;
using SocksShoppingStore.Helpers;
using SocksShoppingStore.Data; // Подключаем наш новый репозиторий

namespace SocksShoppingStore.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            var cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();
            return View(cart);
        }

        public IActionResult AddToCart(int id)
        {
            var cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();
            // Получаем товар из единого репозитория
            var sock = ProductRepository.GetSockById(id);
            if (sock != null)
            {
                cart.AddItem(sock);
                HttpContext.Session.Set("Cart", cart);
            }

            // Если мы добавляем товар из корзины (увеличиваем количество), то остаемся в корзине
            if (Request.Headers["Referer"].ToString().Contains("/Cart"))
            {
                return RedirectToAction("Index");
            }

            // Если добавляем с главной, остаемся на главной
            return RedirectToAction("Index", "Home");
        }

        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.RemoveItem(id);
            HttpContext.Session.Set("Cart", cart);
            return RedirectToAction("Index"); // Обновляем страницу корзины
        }

        public IActionResult DeleteItem(int id)
        {
            var cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.DeleteItem(id);
            HttpContext.Session.Set("Cart", cart);
            return RedirectToAction("Index"); // Обновляем страницу корзины
        }
    }
}