using Microsoft.AspNetCore.Mvc;

namespace SLib23T1020520.WebApp.Controllers
{
    public class BookController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
