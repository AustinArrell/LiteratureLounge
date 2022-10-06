using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LiteratureLounge.Models;


namespace LiteratureLounge.Tools
{
    public class BookLookup
    {
        static readonly HttpClient client = new HttpClient();

        public async Task<Book> LookupBookDetails(string isbn) 
        {
            string responseBody = await RequestBookDetails(isbn);
            JToken bookData = ParseJsonResponse(responseBody);
            return BuildNewBook(isbn, bookData);
        }

        private async Task<string> RequestBookDetails(string isbn) 
        {
            string url = @$"https://www.googleapis.com/books/v1/volumes?q={isbn}";
            HttpResponseMessage response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        private JToken ParseJsonResponse(string responseBody) 
        {
            JObject responseObject = JObject.Parse(responseBody);
            JToken bookList = responseObject.SelectToken("items");
            return bookList.FirstOrDefault();
        }

        private Book BuildNewBook(string isbn, JToken bookData) {
            Book book = new Book();
            book.ISBN = isbn;
            book.Author = (string)bookData["volumeInfo"]["authors"].First;
            book.Title = (string)bookData["volumeInfo"]["title"];
            book.PublishedDate = (string)bookData["volumeInfo"]["publishedDate"];
            book.Subtitle = (string)bookData["volumeInfo"]["subtitle"];
            book.Description = (string)bookData["volumeInfo"]["description"];
            book.Publisher = (string)bookData["volumeInfo"]["publisher"];
            return book;
        }
    }
}
