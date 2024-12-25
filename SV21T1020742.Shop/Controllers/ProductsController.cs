using Microsoft.AspNetCore.Mvc;

namespace SV21T1020742.Shop.Controllers
{
    public class ProductsController : Controller
    {
     
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ProductsDetail()
        {
            return View();
        }
    }
}
