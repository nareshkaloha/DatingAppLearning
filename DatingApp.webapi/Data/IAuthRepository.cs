using System.Threading.Tasks;
using DatingApp.webapi.Model;

namespace DatingApp.webapi.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User user, string password);
         Task<User> Login(string username, string password);
         Task<bool> UserExists(string username);
    }
}