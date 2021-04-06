using LCG.Template.Common.Data.Contracts;
using LCG.Template.Common.Entities.Base;

namespace LCG.Template.Common.Entities.Application
{
    public class EmailTemplate : EntityBase, IIdentifiableEntity<int>
    {
        public int EmailNameId { get; set; }
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Parameters { get; set; }
        public EmailTemplateName EmailName { get; set; }
        public Language Language { get; set; }
    }
}
