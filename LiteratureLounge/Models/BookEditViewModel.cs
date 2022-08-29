namespace LiteratureLounge.Models
{
    public class BookEditViewModel
    {
        public Book book { get; set; }
        public string genreName { get; set; }

        public IEnumerable<Genre> Genres { get; set; } = new List<Genre>();
    }
}
