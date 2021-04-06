using System.ComponentModel.DataAnnotations;

namespace LCG.Template.Common.Models.Account
{
    public class LoginModel
    {
        [Required(ErrorMessageResourceName = "EmailAddressRequired", ErrorMessageResourceType = typeof(Resources.Messages.ValidationsMessages))]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}
