using LCG.Template.Common.Entities.Application;
using LCG.Template.Data.Application.Contracts;
using System.Linq;

namespace LCG.Template.Data.Application.Repositories
{
    public class LanguageRepository : RepositoryBase<Language>, ILanguageRepository
    {
        public LanguageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<Language> GetAllActiveLanguages()
        {
            return Get(x => x.Active);
        }
    }
}
