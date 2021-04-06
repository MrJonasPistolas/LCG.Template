using LCG.Template.Common.Entities.Application;
using LCG.Template.Data.Application.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LCG.Template.Data.Application
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await Context.UserSet.FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
