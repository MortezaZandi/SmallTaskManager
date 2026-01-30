using Microsoft.AspNetCore.Mvc;

namespace SmallTask.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
