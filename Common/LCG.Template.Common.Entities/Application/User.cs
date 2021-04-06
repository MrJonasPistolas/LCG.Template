using LCG.Template.Common.Data.Contracts;
using LCG.Template.Common.Entities.Base;
using System.Collections.Generic;

namespace LCG.Template.Common.Entities.Application
{
    public class User : EntityBase, IIdentifiableEntity<int>
    {
        public string Email { get; set; }
        public string Name { get; set; }

        public string FirstName { get; set; }

        public string MiddleNames { get; set; }

        public string LastName { get; set; }

        public string CompanyName { get; set; }

        public string TaxNumber { get; set; }

        public string PhoneNumber { get; set; }
        public string ImageUrl { get; set; }
        public string LayoutPreferences { get; set; }

        #region Members
        public virtual ICollection<AccountUser> AccountUsers { get; set; }
        #endregion
    }
}
