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
        public string? Notes { get; set; } = "";
        public string? ChapterLength { get; set; } = "Unknown/Other";
        public bool isStamped { get; set; } = false;
        public bool isAnnotated { get; set; } = false;
        public bool isCheckedOut { get; set; } = false;
        public bool isSigned { get; set; } = false;
        public string? ReadStatus { get; set; } = "Unread";
        public string? MediaType { get; set; } = "Unknown";
        public string? CoverLink { get; set; } = "";
        public int? Rating { get; set; } = 0;
    }
}
