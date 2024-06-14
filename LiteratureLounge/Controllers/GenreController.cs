using Microsoft.AspNetCore.Mvc;
using LiteratureLounge.Models;
using System.Diagnostics;
using Purrs_And_Prose.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            IEnumerable<Genre> genres = _db.Genres.Where(g => g.Owner == userId).ToList();
            return View(genres);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Genre genre)
        { 
            if (ModelState.IsValid) 
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                genre.Owner = userId;
                var _genres = _db.Genres.Where(g => g.Owner == userId).ToList();
                foreach (var _genre in _genres)
                {
                    if (_genre.Name == genre.Name) 
                    {
                        TempData["Error"] = "Failed to Create Duplicate Genre";
                        return View();
                    }
                }
                
                _db.Genres.Add(genre);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Created Genre Successfully";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Failed to Create Genre!";
            return View();
        }

        public IActionResult Delete(int? id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var genre = _db.Genres.Where(g => g.Id == id && g.Owner == userId).FirstOrDefault();
            if (genre is not null)
            {
                return View(genre);
            }
            TempData["Error"] = "Action Failed. Failed to Find Genre!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Genre genre)
        {
            if (ModelState.IsValid)
            {
                _db.Genres.Remove(genre);
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Deleted Genre {genre.Name} Successfully";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Failed to Delete Genre!";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var genre = _db.Genres.Where(g => g.Id == id && g.Owner == userId).FirstOrDefault();
            if (genre is not null)
            {
                return View(genre);
            }
            TempData["Error"] = "Failed to Edit Genre!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Genre genre)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid) 
            {
                var _genres = _db.Genres.Where(g => g.Owner == userId).ToList();
                foreach (var _genre in _genres)
                {
                    if (_genre.Name == genre.Name)
                    {
                        TempData["Error"] = "Failed to Create Genre with Duplicate Name";
                        return RedirectToAction("Index");
                    }
                }
                genre.Owner = userId;
                _db.Genres.Update(genre);
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Edited Genre {genre.Name} Successfully";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Failed to Edit Genre!";
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}