using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CURDDEMO.Controllers
{
    public class HomeController : Controller
    {
        [Route("Error")]
        public IActionResult Error()
        {
            IExceptionHandlerPathFeature ? IExceptionHandlerPathFeature=HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (IExceptionHandlerPathFeature != null && IExceptionHandlerPathFeature.Error != null )
            {
                ViewBag.ErrorMessage = IExceptionHandlerPathFeature.Error.Message;
            }
            return View();
        }
    }
}
