using Microsoft.AspNetCore.Mvc;
using LiteratureLounge.Models;
using System.Diagnostics;
using Purrs_And_Prose.Data;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace LiteratureLounge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db, ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var Books = _db.Books.Where(b => b.Owner == userId).ToList();
            return View(new HomeIndexViewModel {Books = Books });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}