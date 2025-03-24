using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ErenAliKocaCV.Filters
{
    /// <summary>
    /// Direkt controller/action URL erişimlerini engelleyip ana sayfaya yönlendiren attribute
    /// </summary>
    public class RedirectToHomeAttribute : ActionFilterAttribute
    {
        private readonly bool _enabled;

        public RedirectToHomeAttribute(bool enabled = true)
        {
            _enabled = enabled;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Eğer filtre devre dışı bırakıldıysa kontrolü geç
            if (!_enabled)
            {
                base.OnActionExecuting(context);
                return;
            }

            // Eğer istek bir AJAX isteği ise kontrolü atla
            if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                base.OnActionExecuting(context);
                return;
            }

            // Referrer kontrolü - eğer referrer yoksa (doğrudan URL erişimi) ana sayfaya yönlendir
            var referrer = context.HttpContext.Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referrer))
            {
                // Doğrudan URL erişimi - Ana sayfaya yönlendir
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
            else
            {
                // Normal istek, devam et
                base.OnActionExecuting(context);
            }
        }
    }
} 