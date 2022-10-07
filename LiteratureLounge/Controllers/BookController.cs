using Microsoft.AspNetCore.Mvc;
using LiteratureLounge.Models;
using LiteratureLounge.Tools;
using Purrs_And_Prose.Data;
using System.Diagnostics;
using System.Net;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult Index()
        {
            IEnumerable<Book> books = _db.Books.ToList();
            return View(books);
        }

        public IActionResult Create()
        {
            var genres = _db.Genres.ToList();
            return View(new BookEditViewModel {Genres = genres });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookEditViewModel model)
        {
            if (model.book.Rating > 5)
                model.book.Rating = 5;
            if (model.book.Rating < 0)
                model.book.Rating = 0;

            var ISBNs = _db.Books.Select(c => c.ISBN).ToList();
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
                var _genre = _db.Genres.Where(g => g.Name == genre).FirstOrDefault();
                var bg = new BookGenre { Genre = _genre };
                model.book.BookGenres.Add(bg);
            }

            _db.Books.Add(model.book);
            _db.SaveChanges();
            TempData["Success"] = $"Added book successfully!";
            return RedirectToAction("DetailedView", new { model.book.Id });
        }

        public IActionResult CreateFromISBN()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromISBN(ISBNBookCreateViewModel viewModel)
        {
            try
            {
                viewModel.ISBN = CleanISBN(viewModel.ISBN);
                Book book = await booklookup.LookupBookDetails(viewModel.ISBN);
                
                book.CoverLink = Path.Combine(@"/Images/Covers/", $"{book.ISBN}.jpg");
                var ISBNs = _db.Books.Select(c => c.ISBN).ToList();
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
        public IActionResult Edit(int? id)
        {
            var _book = _db.Books
                .Include(book => book.BookGenres)
                .ThenInclude(bg => bg.Genre)
                .FirstOrDefault(c => c.Id == id);
            if (_book == null)
            {
                return RedirectToAction("Index");
            }

            var _genreNames = new List<string>();
            foreach (var _bg in _book.BookGenres) 
            {
                _genreNames.Add(_bg.Genre.Name);
            }

            var _seriesNames = new List<string>();
            foreach (var _bs in _db.Books.Select(row => row.Series).ToArray()) 
            {
                if (_bs is not null) 
                {
                    if (!_seriesNames.Contains(_bs)) 
                    {
                        _seriesNames.Add(_bs);
                    }
                }
            }

            var genres = _db.Genres.ToList();
            var model = new BookEditViewModel { book = _book, Genres = genres, genreNames = _genreNames, SeriesNames = _seriesNames};
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookEditViewModel model)
        {
            if (model.book.Rating > 5)
                model.book.Rating = 5;
            if (model.book.Rating < 0)
                model.book.Rating = 0;

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

        public IActionResult Rating(int? rating, int? id)
        {
            var book = _db.Books.FirstOrDefault(c => c.Id == id);
            if (book == null)
            {
                return RedirectToAction("Index");
            }
            book.Rating = rating;
            _db.Update(book);
            _db.SaveChanges();
            return RedirectToAction("DetailedView", new { book.Id });
        }

        public IActionResult DetailedView(int? id)
        {
            var book = _db.Books
                .Include(book => book.BookGenres)
                .ThenInclude(bg => bg.Genre)
                .FirstOrDefault(c => c.Id == id);

            if (book == null)
            {
                return RedirectToAction("Index");
            }

            BookDetailedViewModel viewModel = new BookDetailedViewModel();
            viewModel.book = book;
            return View(viewModel);
        }

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
        public IActionResult Delete(Book book)
        {
            _db.Remove(book);
            _db.SaveChanges();
            TempData["Success"] = $"Removed book successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
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

                // Toastr Alert and Redirect
                TempData["Success"] = $"Uploaded New Cover Image Successfully!";
                return RedirectToAction("DetailedView", new { id });
            }
            // Toastr Alert and Redirect
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
