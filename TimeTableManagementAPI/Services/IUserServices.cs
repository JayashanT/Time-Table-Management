using System.Data;

namespace TimeTableManagementAPI.Services
{
    public interface IUserServices
    {
        DataSet GetAll(string table);
        bool Add(string Name, string Staff_Id, string Contact_No, string Password, int Role_Id);
    }
}