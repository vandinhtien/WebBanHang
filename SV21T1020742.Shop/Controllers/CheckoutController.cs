using Microsoft.AspNetCore.Mvc;

namespace SV21T1020742.Shop.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
