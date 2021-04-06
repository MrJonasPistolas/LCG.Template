using LCG.Template.Common.Entities.Application;
using LCG.Template.Data.Application.Contracts;
using System.Linq;

namespace LCG.Template.Data.Application.Repositories
{
    public class EmailTemplateRepository : RepositoryBase<EmailTemplate>, IEmailTemplateRepository
    {
        public EmailTemplateRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<EmailTemplate> GetEmailTemplateByLanguageAndName(int languageId, int emailNameId)
        {
            return Get().Where(x => x.LanguageId == languageId && x.EmailNameId == emailNameId);
        }

        public IQueryable<EmailTemplate> GetActiveEmailTemplateByLanguageAndName(int languageId, int emailNameId)
        {
            return Get().Where(x => x.LanguageId == languageId && x.EmailNameId == emailNameId && x.Active);
        }
    }
}
