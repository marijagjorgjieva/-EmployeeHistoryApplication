using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace EmployeeHistoryApplication.Controllers
{
    public class CultureController : Controller
    {
        public IActionResult SetCulture(string culture, string returnUrl)
        {
            var supportedCultures = new[] { "en-GB", "mk-MK", "sq-AL" };
            if (Array.Exists(supportedCultures, c => c.Equals(culture, StringComparison.OrdinalIgnoreCase)))
            {

                Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

                var cultureInfo = new CultureInfo(culture);
                var currentCulture = CultureInfo.CurrentCulture.Name;
            }

            return Redirect(returnUrl ?? "/");
        }
    }
}
