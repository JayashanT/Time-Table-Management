namespace TimeTableManagementAPI.Services
{
    public interface IUserServices
    {
        bool Add(string Name, string Staff_Id, string Contact_No, string Password, int Role_Id);
        bool UpdateUser(string Name, string Staff_Id, string Contact_No, string Password, int Role_Id);
    }
}