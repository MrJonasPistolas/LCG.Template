using LCG.Template.Common.Entities.Application;
using System.Linq;

namespace LCG.Template.Data.Application.Contracts
{
    public interface ILanguageRepository
    {
        IQueryable<Language> GetAllActiveLanguages();
    }
}
