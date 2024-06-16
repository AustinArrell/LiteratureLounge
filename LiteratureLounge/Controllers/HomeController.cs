using Microsoft.AspNetCore.Mvc;
using LiteratureLounge.Models;
using System.Diagnostics;
using Purrs_And_Prose.Data;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Extensions;

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

        public IActionResult Index()
        {
            var userId = "DEVUSER";
            var Books = _db.Books.Where(b => b.Owner == userId).ToList();
            return View(new HomeIndexViewModel {Books = Books });
        }

        public IActionResult Calendar()
        {
            var userId = "DEVUSER";
            var Books = _db.Books.Where(b => b.Owner == userId).ToList();
            List<Dictionary<String, String>> readDates = new List<Dictionary<String, String>>(); 
            foreach (var book in Books)
            {
                Dictionary<String, String> item = new Dictionary<string, string>();
                if (book.ReadDate is not null)
                {
                    item.Add("title", book.Title);
                    item.Add("start", book.ReadDate);
                    string url = HttpContext.Request.GetDisplayUrl();
                    url = url.Replace("/Home/Calendar", $"/Book/DetailedView?id={book.Id}");
                    item.Add("url", url);
                    readDates.Add(item);
                }
            }
            return View(new HomeIndexViewModel { Books = Books, ReadDates = readDates });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}