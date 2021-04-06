using LCG.Template.Common.Data.Contracts;
using LCG.Template.Common.Entities.Base;
using System.Collections.Generic;

namespace LCG.Template.Common.Entities.Application
{
    public class Language : EntityBase, IIdentifiableEntity<int>
    {
        public string Description { get; set; }
        public string Code { get; set; }

        #region Members
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<EmailTemplate> EmailTemplates { get; set; }
        #endregion
    }
}
