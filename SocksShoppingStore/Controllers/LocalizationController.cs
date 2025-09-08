using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace SocksShoppingStore.Controllers
{
    public class LocalizationController : Controller
    {
        [HttpPost]
        public IActionResult Set(string culture, string returnUrl)
        {
            var normalized = culture?.StartsWith("ru", StringComparison.OrdinalIgnoreCase) == true ? "ru" : "en";
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(normalized)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true, HttpOnly = false }
            );
            if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl)) returnUrl = Url.Action("Index", "Home")!;
            return LocalRedirect(returnUrl);
        }
    }
}

