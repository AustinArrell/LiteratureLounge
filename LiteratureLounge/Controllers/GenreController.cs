using Microsoft.AspNetCore.Mvc;
using LiteratureLounge.Models;
using System.Diagnostics;
using Purrs_And_Prose.Data;

namespace LiteratureLounge.Controllers
{
    public class GenreController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public GenreController(ApplicationDbContext db, ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index()
        {
            IEnumerable<Genre> genres = _db.Genres.ToList();
            return View(genres);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Genre genre)
        {
            if (genre is not null) 
            {
                _db.Genres.Add(genre);
                _db.SaveChanges();
                TempData["Success"] = "Created Genre Successfully";
                return RedirectToAction("Index");
            }
            TempData["Success"] = "Created Genre Successfully";
            return View();
        }

        public IActionResult Edit(int? id)
        {
            var genre = _db.Genres.Where(g => g.Id == id).FirstOrDefault();
            if (genre is not null)
            {
                return View(genre);
            }
            TempData["Error"] = "Failed to Edit Genre - Genre Null";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Genre genre)
        {
            if (genre is not null) 
            {
                _db.Genres.Update(genre);
                _db.SaveChanges();
                TempData["Success"] = $"Edited Genre {genre.Name} Successfully";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Failed to Edit Genre - Genre Null";
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}