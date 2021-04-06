using System.ComponentModel.DataAnnotations;

namespace LCG.Template.Common.Models.User
{
    public class ConfirmNewUserInputModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string MiddleNames { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string TaxNumber { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string ConfirmationToken { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
