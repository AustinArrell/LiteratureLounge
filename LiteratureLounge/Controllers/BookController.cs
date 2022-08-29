using Microsoft.AspNetCore.Mvc;
using LiteratureLounge.Models;
using LiteratureLounge.Tools;
using Purrs_And_Prose.Data;
using System.Diagnostics;
using System.Net;
using System.Web;

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

        // CREATE
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            if (ModelState.IsValid)
            {
                if (book.Rating > 5)
                    book.Rating = 5;
                if (book.Rating < 0)
                    book.Rating = 0;
                var ISBNs = _db.Books.Select(c => c.ISBN).ToList();
                foreach (var _isbn in ISBNs) 
                {
                    if (_isbn == book.ISBN) 
                    {
                        TempData["Error"] = $"Book has duplicate ISBN!";
                        return RedirectToAction("Create");
                    }
                }

                _db.Books.Add(book);
                _db.SaveChanges();
                TempData["Success"] = $"Added book successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                       .Where(y => y.Count > 0)
                                       .ToList();
                TempData["Error"] = $"Failed to add book!";
                return View(book);
            }
        }
        public IActionResult CreateFromISBN()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromISBN(ISBNBookCreateViewModel viewModel)
        {
            viewModel.ISBN = CleanISBN(viewModel.ISBN);
            Book book = await booklookup.LookupBookDetails(viewModel.ISBN);
            if (!System.IO.File.Exists($"wwwroot/Images/Covers/{viewModel.ISBN}.jpg"))
            {
                WebClient webClient = new WebClient();

                webClient.DownloadFile(@$"https://pictures.abebooks.com/isbn/{viewModel.ISBN}.jpg", $"wwwroot/Images/Covers/{viewModel.ISBN}.jpg");
                webClient.Dispose();
            }

            try
            {
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
                TempData["Error"] = $"Failed to add book! {e.Message}";
            }
            return RedirectToAction("Index");
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
            var _book = _db.Books.FirstOrDefault(c => c.Id == id);
            if (_book == null)
            {
                return RedirectToAction("Index");
            }
            var genres = _db.Genres.ToList();
            var model = new BookEditViewModel { book = _book, Genres = genres };
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
            var book = _db.Books.FirstOrDefault(c => c.Id == id);
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
