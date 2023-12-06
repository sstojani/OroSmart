using System.ComponentModel.DataAnnotations;

namespace OroSmart.Data.ViewModels
{
    public class LoginVM
    {
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email Address is Required")]
        public string? EmailAddress { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

    }
}
