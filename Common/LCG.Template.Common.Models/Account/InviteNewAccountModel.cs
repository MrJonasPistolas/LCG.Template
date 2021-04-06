using System.ComponentModel.DataAnnotations;

namespace LCG.Template.Common.Models.Account
{
    public class InviteNewAccountModel
    {
        public string FirstName { get; set; }
        public string MiddleNames { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string TaxNumber { get; set; }
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        public string AccountName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }
}
