namespace LiteratureLounge.Models
{
    public class HomeIndexViewModel
    {
        public List<Book> Books { get; set; } = new List<Book>();

        public List<Dictionary<String, String>> ReadDates { get; set; } = new List<Dictionary<String, String>>();
    }
}
