using LCG.Template.Common.Models.User;
using System.Collections.Generic;

namespace LCG.Template.Common.Models.Account
{
    public class AccountDetailsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public bool Active { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Language { get; set; }
        public string Website { get; set; }
        public string ImageUrl { get; set; }
        public string Owners { get; set; }
        public string Email { get; set; }
        public string Nif { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        public string AccountOwner { get; set; }
        public IEnumerable<UserDetailsModel> Users { get; set; }
    }
}
