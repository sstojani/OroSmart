using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace OroSmart.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }
    }
}
