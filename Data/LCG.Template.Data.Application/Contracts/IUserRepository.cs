using LCG.Template.Common.Entities.Application;
using System.Threading.Tasks;

namespace LCG.Template.Data.Application
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int userId);
    }
}
