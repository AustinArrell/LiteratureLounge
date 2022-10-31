using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LiteratureLounge.Models
{
    public class IndexColumn
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<UserPreferencesBookIndexColumn>? UserPreferencesBookIndexColumns { get; set; }
    }
}
