using System.ComponentModel.DataAnnotations;

namespace LiteratureLounge.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<BookGenre> BookGenres { get; set; }
    }
}
