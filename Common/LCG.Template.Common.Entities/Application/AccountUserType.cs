using LCG.Template.Common.Data.Contracts;
using LCG.Template.Common.Entities.Base;
using System.Collections.Generic;

namespace LCG.Template.Common.Entities.Application
{
    public class AccountUserType : EntityBase, IIdentifiableEntity<int>
    {
        public string Description { get; set; }

        #region Members
        public virtual ICollection<AccountUser> AccountUsers { get; set; }
        #endregion
    }
}
