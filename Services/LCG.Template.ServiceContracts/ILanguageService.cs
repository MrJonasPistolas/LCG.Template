using LCG.Template.Common.Models.Application;
using System.Collections.Generic;

namespace LCG.Template.ServiceContracts
{
    public interface ILanguageService
    {
        public IEnumerable<LanguageViewModel> GetLanguageByAccount(int? accountId);
    }
}
