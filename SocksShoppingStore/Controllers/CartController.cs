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

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer) && referer.Contains("/Cart"))
            {
                return RedirectToAction("Index");
            }

            // Default to staying within the cart when context is unclear
            return RedirectToAction("Index");
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
