using System.ComponentModel.DataAnnotations;

namespace LiteratureLounge.Models
{
    public class ISBNBookCreateViewModel
    {
        [Required]
        public string ISBN { get; set; }
    }
}
