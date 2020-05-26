using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableManagementAPI.Services
{
    public class UserServices : IUserServices
    {
        string ConnectionInformation = "Server=localhost;Database=TimeTableDB;Trusted_Connection=True;MultipleActiveResultSets=true";
        SqlConnection MainConnection;

        public UserServices()
        {
            MainConnection = new SqlConnection(ConnectionInformation);
            MainConnection.Open();
        }

        public bool Add(string Name,string Staff_Id,string Contact_No,string Password,int Role_Id)
        {
            string MyCommand = "INSERT INTO Users VALUES(" + Name + "," + Staff_Id + "," + Contact_No + "," + Password + "," + Role_Id + ")";
            try
            {
                SqlCommand myCommand = new SqlCommand(MyCommand, MainConnection);
                myCommand.ExecuteNonQuery();
                return true;
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool UpdateUser(string Name, string Staff_Id, string Contact_No, string Password, int Role_Id)
        {
            string MyCommand = "INSERT INTO Users VALUES(" + Name + "," + Staff_Id + "," + Contact_No + "," + Password + "," + Role_Id + ")";
            try
            {
                SqlCommand myCommand = new SqlCommand(MyCommand, MainConnection);
                myCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
