using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace LiteratureLounge.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Subtitle { get; set; }
        public string Author { get; set; }
        public string? Owner { get; set; }
        public string? ISBN { get; set; }
        public string? Publisher { get; set; }
        public string? Description { get; set; }
        public int? PageCount { get; set; }
        public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
        public string? Notes { get; set; } = "";
        public string? Series { get; set; }
        public string? ReadDate { get; set; }
        public string? CatalogDate { get; set; }
        public string? ChapterLength { get; set; } = "Medium";
        public bool isStamped { get; set; } = false;
        public bool isAnnotated { get; set; } = false;
        public bool isCheckedOut { get; set; } = false;
        public bool isSigned { get; set; } = false;
        public bool isFavorite { get; set; } = false;
        public string? ReadStatus { get; set; }
        public string? MediaType { get; set; } = "Paperback";
        public string? CoverLink { get; set; } = "";
        public float? Rating { get; set; } = 0;
        public string? PublishedDate { get; set; } = "";
        public string? CheckedOutTo { get; set; } = "";
        public string? SignatureType { get; set; } = "";
    }
}
