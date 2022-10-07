using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LiteratureLounge.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LiteratureLounge.Tools
{
    public class BookLookup
    {
        static readonly HttpClient client = new HttpClient();

        public async Task<Book> LookupBookDetails(string isbn) 
        {
            string responseBody = await RequestBookDetails(isbn);
            JToken bookData = ParseJsonResponse(responseBody, isbn);
            DownloadCover(isbn);
            return BuildNewBook(isbn, bookData);
        }

        private void DownloadCover(string isbn) 
        {
            if (!File.Exists($"wwwroot/Images/Covers/{isbn}.jpg") && isbn is not null)
            {
                try
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(@$"https://pictures.abebooks.com/isbn/{isbn}.jpg", $"wwwroot/Images/Covers/{isbn}.jpg");
                    webClient.Dispose();
                }
                catch 
                {
                    
                }
            }
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
                book.Title = (string)bookData["volumeInfo"]["title"];
                book.PublishedDate = (string)bookData["volumeInfo"]["publishedDate"];
                book.Subtitle = (string)bookData["volumeInfo"]["subtitle"];
                book.Description = (string)bookData["volumeInfo"]["description"];
                book.Publisher = (string)bookData["volumeInfo"]["publisher"];
                if (bookData["volumeInfo"]["pageCount"] is not null)
                    book.PageCount = (int)bookData["volumeInfo"]["pageCount"];
            }
            else 
            {
                throw new Exception("Failed to locate book with this ISBN");
            }
            return book;
        }
    }
}
