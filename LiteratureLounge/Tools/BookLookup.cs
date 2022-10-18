using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LiteratureLounge.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using LanguageExt.Common;
using System.ComponentModel.DataAnnotations;

namespace LiteratureLounge.Tools
{
    public class BookLookup
    {
        static readonly HttpClient client = new HttpClient();

        public async Task<Result<Book>> LookupBookDetails(string isbn) 
        {
            string responseBody = await RequestBookDetails(isbn);
            JToken bookData = ParseJsonResponse(responseBody, isbn);
            var book = BuildNewBook(isbn, bookData);

            if (book.Title is null) 
            {
                var validationException = new ValidationException("Book/ISBN not found");
                return new Result<Book>(validationException);
            }
            return book;
        }

        private async Task<string> RequestBookDetails(string isbn) 
        {
            string url = @$"https://www.googleapis.com/books/v1/volumes?q=isbn:{isbn}&orderBy=relevance";
            HttpResponseMessage response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        private JToken ParseJsonResponse(string responseBody, string isbn) 
        {
            JObject responseObject = JObject.Parse(responseBody);
            JToken bookList = responseObject.SelectToken("items");
            if (bookList is not null)
            {
                foreach (var book in bookList)
                {
                    var _isbnList = book["volumeInfo"]["industryIdentifiers"];

                    foreach (var _isbn in _isbnList)
                    {
                        if ((string)_isbn["identifier"] == isbn)
                        {
                            return book;
                        }
                    }
                }
            }
            return null;
        }

        private Book BuildNewBook(string isbn, JToken bookData) {
            Book book = new Book();
            if (bookData is not null)
            {
                book.ISBN = isbn;
                if (bookData["volumeInfo"]["authors"] is not null)
                    book.Author = (string)bookData["volumeInfo"]["authors"].First;
                else
                    book.Author = "Unknown";
                if (bookData["volumeInfo"]["title"] is not null)
                    book.Title = (string)bookData["volumeInfo"]["title"];
                if (bookData["volumeInfo"]["publishedDate"] is not null)
                    book.PublishedDate = (string)bookData["volumeInfo"]["publishedDate"];
                if (bookData["volumeInfo"]["subtitle"] is not null)
                    book.Subtitle = (string)bookData["volumeInfo"]["subtitle"];
                if (bookData["volumeInfo"]["description"] is not null)
                    book.Description = (string)bookData["volumeInfo"]["description"];
                if (bookData["volumeInfo"]["publisher"] is not null)
                    book.Publisher = (string)bookData["volumeInfo"]["publisher"];
                if (bookData["volumeInfo"]["pageCount"] is not null)
                    book.PageCount = (int)bookData["volumeInfo"]["pageCount"];
                if (bookData["volumeInfo"] is not null)
                {
                    string id = (string)bookData["id"];
                    string link = @$"https://books.google.com/books/publisher/content/images/frontcover/{id}?fife=w300-h600&source=gbs_api";
                    book.CoverLink = link;
                }
            }
            return book;
        }
    }
}
