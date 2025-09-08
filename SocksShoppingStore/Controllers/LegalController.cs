using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace SocksShoppingStore.Controllers
{
    public class LegalOptions
    {
        public string ControllerName { get; set; } = "SocksShoppingStore";
        public string ContactEmail { get; set; } = "support@example.com";
    }

    public class LegalController : Controller
    {
        private readonly LegalOptions _options;
        public LegalController(IOptions<LegalOptions> options)
        {
            _options = options.Value;
        }

        [HttpGet("/legal/terms")]
        public IActionResult Terms()
        {
            ViewData["ControllerName"] = _options.ControllerName;
            ViewData["ContactEmail"] = _options.ContactEmail;
            return View();
        }

        [HttpGet("/legal/privacy")]
        public IActionResult Privacy()
        {
            ViewData["ControllerName"] = _options.ControllerName;
            ViewData["ContactEmail"] = _options.ContactEmail;
            return View();
        }
    }
}

