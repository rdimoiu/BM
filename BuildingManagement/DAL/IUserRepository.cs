using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface IUserRepository : IGenericRepository<User>
    {
        User GetUserByUsernameAndPassword(string username, string password);
    }
}