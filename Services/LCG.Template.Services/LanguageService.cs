using LCG.Template.Common.Models.Application;
using LCG.Template.Data.Application;
using LCG.Template.Data.Application.Repositories;
using LCG.Template.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCG.Template.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public LanguageService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IEnumerable<LanguageViewModel> GetLanguageByAccount(int? accountId)
        {
            if (!accountId.HasValue)
            {
                using (var languageRepository = new LanguageRepository(_applicationDbContext))
                {
                    return languageRepository.GetAllActiveLanguages().Select(x => new LanguageViewModel
                    {
                        Id = x.Id,
                        Active = x.Active,
                        Code = x.Code,
                        Description = x.Description
                    });
                }
            }
            using (var accountRepository = new AccountRepository(_applicationDbContext))
            {
                return accountRepository.Get(x => x.Id == accountId).Select(x => new LanguageViewModel
                {
                    Id = x.Language.Id,
                    Active = x.Language.Active,
                    Code = x.Language.Code,
                    Description = x.Language.Description
                });
            }
        }
    }
}
