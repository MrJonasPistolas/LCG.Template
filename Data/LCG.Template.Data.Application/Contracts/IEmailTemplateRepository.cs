using LCG.Template.Common.Entities.Application;
using System.Linq;

namespace LCG.Template.Data.Application.Contracts
{
    public interface IEmailTemplateRepository
    {
        IQueryable<EmailTemplate> GetEmailTemplateByLanguageAndName(int languageId, int emailNameId);

        IQueryable<EmailTemplate> GetActiveEmailTemplateByLanguageAndName(int languageId, int emailNameId);
    }
}
