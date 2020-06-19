using System.ComponentModel.DataAnnotations;

namespace FruityNET.Models
{
    public class ResetPasswordModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password",
        ErrorMessage = "Password and Confirmation do not match")]
        public string ConfirmPassword { get; set; }
    }
}