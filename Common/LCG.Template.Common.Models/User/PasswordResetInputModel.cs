using System.ComponentModel.DataAnnotations;

namespace LCG.Template.Common.Models.User
{
    public class PasswordResetInputModel
    {
        [Required]
        public string ConfirmationToken { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
