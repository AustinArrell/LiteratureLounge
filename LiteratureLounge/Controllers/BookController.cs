using Microsoft.AspNetCore.Mvc;
using LiteratureLounge.Models;
using LiteratureLounge.Tools;
using Purrs_And_Prose.Data;
using System.Diagnostics;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        public IActionResult Index()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            IEnumerable<Book> books = _db.Books.Where(b => b.Owner == userId).ToList().OrderBy(b => b.Title);
            return View(books);
        }

        [Authorize]
        public IActionResult Create()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var genres = _db.Genres.Where(g => g.Owner == userId).ToList();
            return View(new BookEditViewModel {Genres = genres });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookEditViewModel model)
        {
            if (model.book.Rating > 5)
                model.book.Rating = 5;
            if (model.book.Rating < 0)
                model.book.Rating = 0;

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            model.book.Owner = userId;
            var ISBNs = _db.Books.Where(b=> b.Owner == userId).Select(b => b.ISBN).ToList();
            foreach (var _isbn in ISBNs) 
            {
                if (_isbn == model.book.ISBN) 
                {
                    TempData["Error"] = $"Book has duplicate ISBN!";
                    return RedirectToAction("Create");
                }
            }
            
            foreach (var genre in model.genreNames)
            {
                var _genre = _db.Genres.Where(g => g.Owner == userId).Where(g => g.Name == genre).FirstOrDefault();
                var bg = new BookGenre { Genre = _genre };
                model.book.BookGenres.Add(bg);
            }
            
            _db.Books.Add(model.book);
            _db.SaveChanges();
            TempData["Success"] = $"Added book successfully!";
            return RedirectToAction("DetailedView", new { model.book.Id });
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
            try
            {
                viewModel.ISBN = CleanISBN(viewModel.ISBN);
                Book book = await booklookup.LookupBookDetails(viewModel.ISBN);
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                
                book.Owner = userId;
                book.CoverLink = Path.Combine(@"/Images/Covers/", $"{book.ISBN}.jpg");
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
                return RedirectToAction("Edit", new { book.Id });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                TempData["Error"] = e.Message;
            }
            return RedirectToAction("CreateFromISBN");
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

            // Populate BookGenres currently attached to Book
            var _genreNames = new List<string>();
            foreach (var _bg in _book.BookGenres) 
            {
                _genreNames.Add(_bg.Genre.Name);
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

            // Populate genre list dropdown from database
            var genres = _db.Genres.Where(g => g.Owner == userId).ToList();
            var model = new BookEditViewModel { book = _book, Genres = genres, genreNames = _genreNames, SeriesNames = _seriesNames};
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Edit(BookEditViewModel model)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Block attempts to edit books not owned by current user
            if (model.book.Owner != userId) 
            {
                TempData["Error"] = $"Edit Failed! Cannot find book to edit!";
                return RedirectToAction("Index");
            }
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
            _db.SaveChanges();

            foreach (var genre in model.genreNames) 
            {
                var _genre = _db.Genres.Where(g => g.Name == genre).FirstOrDefault();
                var bg = new BookGenre { Genre = _genre};
                model.book.BookGenres.Add(bg);
            }

            _db.Books.Update(model.book);
            _db.SaveChanges();
            TempData["Success"] = $"Edited Book: {model.book.Title} successfully!";
            return RedirectToAction("DetailedView", new { model.book.Id });
        }

        [Authorize]
        public IActionResult Rating(int? rating, int? id)
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
            _db.SaveChanges();
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
            var book = _db.Books.FirstOrDefault(c => c.Id == id);
            if (book == null)
            {
                return RedirectToAction("Index");
            }
            BookDetailedViewModel viewModel = new BookDetailedViewModel();
            viewModel.book = book;
            return View(viewModel);
        }

        [Authorize]
        public IActionResult Delete(int? id)
        {
            var book = _db.Books.FirstOrDefault(c => c.Id == id);
            if (book == null)
            {
                TempData["Error"] = "Failed to remove book!";
                return RedirectToAction("Index");
            }
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Delete(Book book)
        {
            _db.Remove(book);
            _db.SaveChanges();
            TempData["Success"] = $"Removed book successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public IActionResult Upload(BookDetailedViewModel model, string isbn, int id)
        {
            var dbBook = _db.Books.Where(b => b.Id == id).FirstOrDefault();
            if (model.file is not null && dbBook is not null)
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
                var imagePath = Path.Combine(rootPath, $"Images\\Covers\\{filePathTail}");

                // Save Image
                using (var fs = new FileStream(imagePath, FileMode.Create))
                {
                    model.file.CopyTo(fs);
                }

                // Update Book Image
                dbBook.CoverLink = Path.Combine(@"/Images/Covers/", filePathTail);
                _db.Update(dbBook);
                _db.SaveChanges();

                TempData["Success"] = $"Uploaded New Cover Image Successfully!";
                return RedirectToAction("DetailedView", new { id });
            }
            TempData["Error"] = $"No File Detected For Upload!";
            return RedirectToAction("DetailedView", new { id });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
