using System.ComponentModel.DataAnnotations;

namespace LiteratureLounge.Models
{
    public class UserPreferences
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public ICollection<UserPreferencesBookIndexColumn> UserPreferencesBookIndexColumns { get; set; } 
               = new List<UserPreferencesBookIndexColumn>();

    }
}
