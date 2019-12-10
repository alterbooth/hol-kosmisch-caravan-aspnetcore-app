using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MyWebApp.Models;

namespace MyWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDistributedCache cache;

        public HomeController(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public IActionResult Index()
        {
            ViewBag.Message = cache.GetString("message");
            return View();
        }

        [HttpPost]
        public IActionResult Index(MyForm item)
        {
            if (!string.IsNullOrWhiteSpace(item?.Message))
            {
                cache.SetString("message", item.Message);
            }
            return RedirectToAction("Index");
        }
    }
}
