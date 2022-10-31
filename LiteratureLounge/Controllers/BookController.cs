using Microsoft.AspNetCore.Mvc;
using LiteratureLounge.Models;
using LiteratureLounge.Tools;
using Purrs_And_Prose.Data;
using System.Diagnostics;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LanguageExt.TypeClasses;
using Microsoft.AspNetCore.Mvc.Rendering;
using LiteratureLounge.Controller_Extensions;

namespace LiteratureLounge.Controllers
{
    public class BookController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        BookLookup booklookup = new BookLookup();
        private readonly IWebHostEnvironment hostingEnvironment;

        public BookController(ILogger<HomeController> logger, ApplicationDbContext db, IWebHostEnvironment enviornment)
        {
            _db = db;
            _logger = logger;
            hostingEnvironment = enviornment;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userPrefs = _db.UserPreferences
                .Where(up => up.UserId == userId)
                .Include(up => up.UserPreferencesBookIndexColumns)
                .ThenInclude(upic => upic.IndexColumn)
                .FirstOrDefault();

            if (userPrefs is null)
            {
                await BookControllerExtensions.SetupDefaultUserPrefs(_db, userId);
                userPrefs = _db.UserPreferences
                    .Where(up => up.UserId == userId)
                    .Include(up => up.UserPreferencesBookIndexColumns)
                    .ThenInclude(upic => upic.IndexColumn)
                    .FirstOrDefault();
            }
                
            IEnumerable<Book> books = _db.Books.Where(b => b.Owner == userId).ToList().OrderBy(b => b.Title);
            return View(new BookIndexViewModel { Books = books, UserPreferencesBookIndexColumns = userPrefs.UserPreferencesBookIndexColumns});
        }

        [Authorize]
        public IActionResult Create()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var genres = _db.Genres.Where(g => g.Owner == userId).ToList();
            MultiSelectList _genreList = new MultiSelectList(genres, nameof(Genre.Id), nameof(Genre.Name));
            return View(new BookEditViewModel {GenreSelectItems=_genreList});
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookEditViewModel model)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            model.book.Owner = userId;
            model.book.CatalogDate = DateTime.Today.ToString();

            if (ModelState.IsValid)
            {
                if (model.book.Rating > 5)
                    model.book.Rating = 5;
                if (model.book.Rating < 0)
                    model.book.Rating = 0;

                var ISBNs = _db.Books.Where(b => b.Owner == userId).Select(b => b.ISBN).ToList();
                foreach (var _isbn in ISBNs)
                {
                    if (_isbn == model.book.ISBN)
                    {
                        TempData["Error"] = $"Book has duplicate ISBN!";
                        return RedirectToAction("Create");
                    }
                }

                foreach (var genreId in model.genreIds)
                {
                    var _genre = _db.Genres.Where(g => g.Owner == userId).Where(g => g.Id == genreId).FirstOrDefault();
                    var bg = new BookGenre { Genre = _genre };
                    model.book.BookGenres.Add(bg);
                }

                _db.Books.Add(model.book);
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Added book successfully!";
                return RedirectToAction("DetailedView", new { model.book.Id });
            }
            else {
                TempData["Error"] = $"Failed to add Book! ";
                return RedirectToAction("Create");
            }
        }

        [Authorize]
        public IActionResult CreateFromISBN()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CreateFromISBN(ISBNBookCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.ISBN = CleanISBN(viewModel.ISBN);
                var result = await booklookup.LookupBookDetails(viewModel.ISBN);
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                return result.Match(book =>
                {
                    book.Owner = userId;
                    var ISBNs = _db.Books.Where(b => b.Owner == userId).Select(c => c.ISBN).ToList();
                    foreach (var _isbn in ISBNs)
                    {
                        if (_isbn == book.ISBN)
                        {
                            TempData["Error"] = $"Book has duplicate ISBN!";
                            return RedirectToAction("CreateFromISBN");
                        }
                    }
                    _db.Books.Add(book);
                    _db.SaveChanges();
                    TempData["Success"] = $"Added book successfully!";
                    return RedirectToAction("Edit", new { Id = book.Id });

                }, exception =>
                {
                    TempData["Error"] = exception.Message;
                    return RedirectToAction("CreateFromISBN");
                });

            }
            else {
                TempData["Error"] = "Failed to create book!";
                return RedirectToAction("CreateFromISBN");
            }
        }

        private string CleanISBN(string isbn)
        {
            string newIsbn = isbn;
            newIsbn = newIsbn.Replace("-", string.Empty);
            newIsbn = newIsbn.Replace("\u200B", string.Empty);
            newIsbn = newIsbn.Replace(" ", string.Empty);
            return newIsbn;
        }

        // UPDATE
        [Authorize]
        public IActionResult Edit(int? id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var _book = _db.Books
                .Where(b => b.Owner == userId)
                .Include(book => book.BookGenres)
                .ThenInclude(bg => bg.Genre)
                .FirstOrDefault(c => c.Id == id);

            if (_book == null)
            {
                TempData["Error"] = $"Edit Failed! Cannot find book to edit!";
                return RedirectToAction("Index");
            }

            // Populate dropdown for series names from existing books
            var _seriesNames = new List<string>();
            foreach (var _bs in _db.Books.Where(b => b.Owner == userId).Select(row => row.Series).ToArray()) 
            {
                if (_bs is not null) 
                {
                    if (!_seriesNames.Contains(_bs)) 
                    {
                        _seriesNames.Add(_bs);
                    }
                }
            }

            List<int> currentGenreIds = new List<int>();
            foreach (var bg in _book.BookGenres) 
            {
                currentGenreIds.Add(bg.Genre.Id);
            }

            var genres = _db.Genres.Where(g => g.Owner == userId).ToList();
            MultiSelectList _genreList = new MultiSelectList(genres, nameof(Genre.Id), nameof(Genre.Name), currentGenreIds);

            var model = new BookEditViewModel { book = _book, genreIds = currentGenreIds, GenreSelectItems = _genreList, SeriesNames = _seriesNames};
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(BookEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.book.Rating > 5)
                    model.book.Rating = 5;
                if (model.book.Rating < 0)
                    model.book.Rating = 0;

                // If old genres are not removed then they would just keep stacking on edit
                var oldGenres = _db.BookGenres.Where(bg => bg.BookId == model.book.Id).ToList();
                foreach (var genre in oldGenres)
                {
                    _db.BookGenres.Remove(genre);
                }
                await _db.SaveChangesAsync();

                foreach (var genre in model.genreIds)
                {
                    var _genre = _db.Genres.Where(g => g.Id == genre).FirstOrDefault();
                    var bg = new BookGenre { Genre = _genre };
                    model.book.BookGenres.Add(bg);
                }

                _db.Books.Update(model.book);
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Edited Book: {model.book.Title} successfully!";
                return RedirectToAction("DetailedView", new { model.book.Id });
            }
            else 
            {
                if (model.book.Title is null)
                {
                    TempData["Error"] = $"Edit Failed! Book title must contain a value!";
                    return RedirectToAction("Edit", new { model.book.Id });
                }
                if (model.book.Author is null)
                {
                    TempData["Error"] = $"Edit Failed! Book author must contain a value!";
                    return RedirectToAction("Edit", new { model.book.Id });
                }
                TempData["Error"] = $"Edit Failed!";
                return RedirectToAction("Edit", new { model.book.Id });
            }
        }
        [Authorize]
        public async Task<IActionResult> Rating(int? rating, int? id)
        {
            // Block attempts to edit books not owned by current user
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var book = _db.Books.Where(b => b.Owner == userId).FirstOrDefault(c => c.Id == id);
            if (book == null)
            {
                TempData["Error"] = $"Edit Failed! Cannot find book to edit!";
                return RedirectToAction("Index");
            }
            book.Rating = rating;
            _db.Update(book);
            await _db.SaveChangesAsync();
            return RedirectToAction("DetailedView", new { book.Id });
        }

        [Authorize]
        public IActionResult DetailedView(int? id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var book = _db.Books
                .Where(b => b.Owner == userId)
                .Include(book => book.BookGenres)
                .ThenInclude(bg => bg.Genre)
                .FirstOrDefault(c => c.Id == id);

            if (book == null)
            {
                TempData["Error"] = $"Action Failed! Cannot find book!";
                return RedirectToAction("Index");
            }

            BookDetailedViewModel viewModel = new BookDetailedViewModel();
            viewModel.book = book;
            return View(viewModel);
        }

        [Authorize]
        public IActionResult EditCoverImage(int? id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var book = _db.Books.Where(b => b.Owner == userId).FirstOrDefault(c => c.Id == id);
            if (book == null)
            {
                TempData["Error"] = $"Action Failed! Cannot find book!";
                return RedirectToAction("Index");
            }
            BookDetailedViewModel viewModel = new BookDetailedViewModel();
            viewModel.book = book;
            return View(viewModel);
        }

        [Authorize]
        public IActionResult Delete(int? id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var book = _db.Books.Where(b => b.Owner == userId).FirstOrDefault(c => c.Id == id);
            if (book == null)
            {
                TempData["Error"] = $"Action Failed! Cannot find book!";
                return RedirectToAction("Index");
            }
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(Book book)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (book.Owner != userId) 
                {
                    TempData["Error"] = $"Failed to delete book!";
                    return RedirectToAction("Index");
                }
                _db.Remove(book);
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Removed book successfully!";
                return RedirectToAction("Index");
            }
            else 
            {
                TempData["Error"] = $"Failed to delete book!";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Upload(BookDetailedViewModel model, string isbn, int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var dbBook = _db.Books.Where(b => b.Id == id && b.Owner == userId).FirstOrDefault();
            if (model.file is not null && dbBook is not null && ModelState.IsValid)
            {
                // Validate file extension
                if (!UploadTools.IsFileTypeValid(model.file.FileName, new string[] { ".jpg", ".png", ".gif", ".webp" }))
                {
                    TempData["Error"] = $"Failed to Upload Image! File Type Invalid!";
                    return RedirectToAction("EditCoverImage", new { id });
                }
                
                // Setup File Paths
                var filePathTail = $"{model.file.FileName.Split(".").First()}.jpg";
                var rootPath = hostingEnvironment.WebRootPath;
                var imagePath = Path.Combine(rootPath, $"Images\\Covers\\{userId.Replace("|","")}{filePathTail}");

                // Save Image
                using (var fs = new FileStream(imagePath, FileMode.Create))
                {
                    await model.file.CopyToAsync(fs);
                }

                // Update Book Image
                dbBook.CoverLink = Path.Combine(@"/Images/Covers/", userId.Replace("|", "") + filePathTail);
                _db.Update(dbBook);
                await _db.SaveChangesAsync();
                
                TempData["Success"] = $"Uploaded New Cover Image Successfully!";
                return RedirectToAction("DetailedView", new { id });
            }
            TempData["Error"] = $"File upload failed!";
            return RedirectToAction("DetailedView", new { id });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
