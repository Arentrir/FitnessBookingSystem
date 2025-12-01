using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FitnessBookingSystem.Models;
using Microsoft.Extensions.Logging;

namespace FitnessBookingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Главна страница – показва интерактивни карти и информация за сайта
        public IActionResult Index()
        {
            ViewData["BodyClass"] = "home-bg";
            return View();
        }


        // Privacy страница – стандартна
        public IActionResult Privacy()
        {
            return View();
        }

        // Error страница
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(model);
        }

        // Примерен метод за бъдещо динамично съдържание на Home – може да се използва за промоции, новини и др.
        // public IActionResult Promo()
        // {
        //     var promoList = _promoService.GetActivePromotions();
        //     return View(promoList);
        // }
    }
}

