using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TechParser.Core.Data;
using TechParser.Models;

namespace TechParser.Controllers
{
    public class HomeController : Controller
    {
        private readonly ParserDbContext _context;

        public HomeController(ParserDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Providers()
        {
            return View(_context.Providers);
        }

        [HttpGet]
        public IActionResult Clients()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Orders()
        {
            return View(_context.Orders);
        }

        [HttpGet]
        public IActionResult Ratings()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}