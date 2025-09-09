using Microsoft.AspNetCore.Mvc;
using SocksShoppingStore.Helpers;
using SocksShoppingStore.Models;
using Microsoft.Extensions.Logging;

namespace SocksShoppingStore.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ILogger<CheckoutController> _logger;
        public CheckoutController(ILogger<CheckoutController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Show checkout form
            return View(new CheckoutViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CheckoutViewModel model)
        {
            // Honeypot: ignore bots filling hidden field
            if (!string.IsNullOrWhiteSpace(model.Website))
            {
                ModelState.AddModelError(string.Empty, "Invalid submission.");
            }

            // Ensure cart has items
            var cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();
            if (!cart.Items.Any())
            {
                ModelState.AddModelError(string.Empty, "Your cart is empty.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Map to order draft and store in session
            var draft = new Order
            {
                CustomerName = model.CustomerName,
                Email = model.Email,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                City = model.City,
                PostalCode = model.PostalCode,
                Country = model.Country,
                Items = cart.Items.Select(i => new OrderItem
                {
                    ProductId = i.Sock.Id,
                    Name = i.Sock.Name,
                    UnitPrice = i.Sock.Price,
                    Quantity = i.Quantity
                }).ToList()
            };

            HttpContext.Session.Set("OrderDraft", draft);
            _logger.LogInformation("checkout_start: items={Count}", draft.Items.Count);
            return RedirectToAction("Review");
        }

        [HttpGet]
        public IActionResult Review()
        {
            var draft = HttpContext.Session.Get<Order>("OrderDraft");
            if (draft == null) return RedirectToAction("Index");
            return View(draft);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Confirm()
        {
            var draft = HttpContext.Session.Get<Order>("OrderDraft");
            if (draft == null) return RedirectToAction("Index");

            // Finalize (no persistence in demo)
            draft.Id = Guid.NewGuid();
            draft.CreatedAt = DateTimeOffset.UtcNow;

            // Clear cart and draft; keep last order for ThankYou page
            HttpContext.Session.Set("LastOrder", draft);
            HttpContext.Session.Set("Cart", new ShoppingCart());
            HttpContext.Session.Set<Order>("OrderDraft", null!);

            _logger.LogInformation("checkout_confirmed: order={OrderId} total={Total}", draft.Id, draft.Total);
            return RedirectToAction("ThankYou");
        }

        [HttpGet]
        public IActionResult ThankYou()
        {
            var order = HttpContext.Session.Get<Order>("LastOrder");
            return View(order);
        }
    }
}

