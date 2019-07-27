using BuildingManagement.Models;
using System.Linq;

namespace BuildingManagement.DAL
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public User GetUserByUsernameAndPassword(string username, string password)
        {
            string encryptedPassword = Cryptography.SimpleAes.Encrypt(password);
            return MainContext.Users.SingleOrDefault(item => item.Email == username && item.Password == encryptedPassword);
        }
    }
}