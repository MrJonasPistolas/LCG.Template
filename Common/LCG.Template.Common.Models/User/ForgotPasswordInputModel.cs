using System.ComponentModel.DataAnnotations;

namespace LCG.Template.Common.Models.User
{
    public class ForgotPasswordInputModel
    {
        [Required]
        public string Email { get; set; }
    }
}
