using LCG.Template.Common.Enums.Entities;

namespace LCG.Template.Common.Models.User
{
    public class UserDetailsModel
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public bool ActiveInAccount { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string MiddleNames { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string TaxNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string LayoutPreferences { get; set; }
        public string ImageUrl { get; set; }
        public string TypeUser { get; set; }
        public string NormalizedName { get { return $"{FirstName} {LastName}"; } }
        public int AccountUserId { get; set; }
        public AccountInviteStates InviteState { get; set; }

        public static explicit operator UserDetailsModel(Entities.Application.User u)
        {
            return new UserDetailsModel
            {
                CompanyName = u.CompanyName,
                Email = u.Email,
                FirstName = u.FirstName,
                Id = u.Id,
                ImageUrl = u.ImageUrl,
                LastName = u.LastName,
                LayoutPreferences = u.LayoutPreferences,
                MiddleNames = u.MiddleNames,
                PhoneNumber = u.PhoneNumber,
                TaxNumber = u.TaxNumber
            };
        }
    }
}
