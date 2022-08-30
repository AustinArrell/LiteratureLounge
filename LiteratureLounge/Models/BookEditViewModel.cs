namespace LiteratureLounge.Models
{
    public class BookEditViewModel
    {
        public Book book { get; set; }
        public string genreName { get; set; }

        public List<string> genreNames { get; set; } = new List<string>();
        public IEnumerable<Genre> Genres { get; set; } = new List<Genre>();
    }
}
