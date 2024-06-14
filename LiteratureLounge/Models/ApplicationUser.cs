using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LiteratureLounge.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Key]
        public int Id { get; set; }
    }
}
