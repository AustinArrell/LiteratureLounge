namespace LiteratureLounge.Models
{
    public class BookIndexViewModel
    {
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<UserPreferencesBookIndexColumn> UserPreferencesBookIndexColumns { get; set; }
    }
}
