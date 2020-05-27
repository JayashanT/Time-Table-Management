using TimeTableAPI.Models;

namespace TimeTableManagementAPI.Services
{
    public interface IUserServices
    {
        bool Add(Users user);
        bool UpdateUser(Users user);
    }
}