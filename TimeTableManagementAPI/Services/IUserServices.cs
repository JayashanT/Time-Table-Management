using TimeTableAPI.Models;

namespace TimeTableManagementAPI.Services
{
    public interface IUserServices
    {
        bool Add(Users user);
        string Decrypt(string password, string keyString);
        string Encrypt(string password, string keyString);
        bool UpdateUser(Users user);
    }
}