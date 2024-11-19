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
                    "Culture",
                    culture,
                    new CookieOptions() { Expires = DateTime.UtcNow.AddYears(1), IsEssential = true });

                var cultureInfo = new CultureInfo(culture);
                CultureInfo.CurrentCulture = cultureInfo;
                CultureInfo.CurrentUICulture = cultureInfo;
                var currentCulture = CultureInfo.CurrentCulture.Name;
            }

            return Redirect(returnUrl ?? "/");
        }
    }
}
