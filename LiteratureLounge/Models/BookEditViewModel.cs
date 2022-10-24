using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LiteratureLounge.Models
{
    public class BookEditViewModel
    {
        public Book book { get; set; }
        public List<string> SeriesNames { get; set; } = new List<string>();
        public List<int> genreIds { get; set; } = new List<int>();
        public MultiSelectList GenreSelectItems { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "Please enter Title")]
        public string Title { get; set; }
        [DisplayName("Author")]
        [Required(ErrorMessage = "Please enter Author")]
        public string Author { get; set; }
    }
}
