using LCG.Template.Common.Data.Contracts;
using LCG.Template.Common.Entities.Base;
using System.Collections.Generic;

namespace LCG.Template.Common.Entities.Application
{
    public class Account : EntityBase, IIdentifiableEntity<int>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string LogoUrl { get; set; }
        public string Address { get; set; }
        public string Nif { get; set; }
        public string PhoneNumber { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Website { get; set; }
        public string ImageUrl { get; set; }
        public int LanguageId { get; set; }

        #region Members
        public virtual Language Language { get; set; }
        public virtual ICollection<AccountUser> AccountUsers { get; set; }
        #endregion
    }
}
