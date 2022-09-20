using System.ComponentModel.DataAnnotations;

namespace LiteratureLounge.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Author { get; set; }
        [Required]
        public string? ISBN { get; set; }

        public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
        public string? Notes { get; set; } = "";
        public string? ChapterLength { get; set; } = "Medium";
        public bool isStamped { get; set; } = false;
        public bool isAnnotated { get; set; } = false;
        public bool isCheckedOut { get; set; } = false;
        public bool isSigned { get; set; } = false;
        public bool isFavorite { get; set; } = false;
        public string? ReadStatus { get; set; } = "Read";
        public string? MediaType { get; set; } = "Paperback";
        public string? CoverLink { get; set; } = "";
        public float? Rating { get; set; } = 0;
        public string? PublishedDate { get; set; } = "";
        public string? CheckedOutTo { get; set; } = "";
        public string? SignatureType { get; set; } = "";
    }
}
