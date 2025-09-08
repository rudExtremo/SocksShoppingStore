using Microsoft.AspNetCore.Mvc;
using SocksShoppingStore.Models;
using SocksShoppingStore.Helpers;
using SocksShoppingStore.Data; // Repository access

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
            var sock = ProductRepository.GetSockById(id);
            if (sock != null)
            {
                cart.AddItem(sock);
                HttpContext.Session.Set("Cart", cart);
            }

            if (Request.Headers["Referer"].ToString().Contains("/Cart"))
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.RemoveItem(id);
            HttpContext.Session.Set("Cart", cart);
            return RedirectToAction("Index");
        }

        public IActionResult DeleteItem(int id)
        {
            var cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.DeleteItem(id);
            HttpContext.Session.Set("Cart", cart);
            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            var cart = new ShoppingCart();
            HttpContext.Session.Set("Cart", cart);
            return RedirectToAction("Index");
        }
    }
}

