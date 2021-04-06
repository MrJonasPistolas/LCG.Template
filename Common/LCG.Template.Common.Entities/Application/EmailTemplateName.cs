using LCG.Template.Common.Data.Contracts;
using LCG.Template.Common.Entities.Base;
using System.Collections.Generic;

namespace LCG.Template.Common.Entities.Application
{
    public class EmailTemplateName : EntityBase, IIdentifiableEntity<int>
    {
        public string Description { get; set; }

        public virtual ICollection<EmailTemplate> EmailTemplates { get; set; }
    }
}
