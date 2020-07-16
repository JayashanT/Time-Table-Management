using TimeTableAPI.Models;

namespace TimeTableManagementAPI.Services
{
    public interface IUserServices
    {
        object Add(Users user);
        string Decrypt(string password, string keyString);
        string Encrypt(string password, string keyString);
        object UpdateUser(Users user);
    }
}