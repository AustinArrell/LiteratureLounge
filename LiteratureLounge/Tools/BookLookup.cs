using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LiteratureLounge.Models;
using Microsoft.AspNetCore.Mvc;

namespace LiteratureLounge.Tools
{
    public class BookLookup
    {
        static readonly HttpClient client = new HttpClient();

        public async Task<Book> LookupBookDetails(string isbn) 
        {
            string responseBody = await RequestBookDetails(isbn);
            JToken bookData = ParseJsonResponse(responseBody, isbn);
            return BuildNewBook(isbn, bookData);
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
                book.Author = (string)bookData["volumeInfo"]["authors"].First;
                book.Title = (string)bookData["volumeInfo"]["title"];
                book.PublishedDate = (string)bookData["volumeInfo"]["publishedDate"];
                book.Subtitle = (string)bookData["volumeInfo"]["subtitle"];
                book.Description = (string)bookData["volumeInfo"]["description"];
                book.Publisher = (string)bookData["volumeInfo"]["publisher"];
                book.PageCount = (int)bookData["volumeInfo"]["pageCount"];
            }
            return book;
        }
    }
}
