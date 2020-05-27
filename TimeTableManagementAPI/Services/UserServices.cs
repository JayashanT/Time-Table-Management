using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TimeTableAPI.Models;

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

        public bool Add(Users user)
        {
            string InsertCommand = "INSERT INTO Users {Name,Staff_Id,Contact_No,Password,Role_Id} VALUES(@Name,@Staff_Id,@Contact_No,@Password,@Role_Id)";
            try
            {
                SqlCommand insertCommand = new SqlCommand(InsertCommand, MainConnection);
                insertCommand.Parameters.AddWithValue("@Name", user.Name);
                insertCommand.Parameters.AddWithValue("@Staff_Id", user.Staff_Id);
                insertCommand.Parameters.AddWithValue("@Contact_No", user.Contact_No);
                insertCommand.Parameters.AddWithValue("@Password", user.Password);
                insertCommand.Parameters.AddWithValue("@Role_Id", user.Role_Id);

                var result = insertCommand.ExecuteNonQuery();
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool UpdateUser(Users user)
        {
            string InsertCommand = "UPDATE Users SET Name=@Name,Staff_Id=@Staff_Id,Contact_No=@Contact_No,Password=@Password,Role_Id=@Role_Id WHERE Id=" + user.Id;
            try
            {
                SqlCommand insertCommand = new SqlCommand(InsertCommand, MainConnection);
                insertCommand.Parameters.AddWithValue("@Name", user.Name);
                insertCommand.Parameters.AddWithValue("@Staff_Id", user.Staff_Id);
                insertCommand.Parameters.AddWithValue("@Contact_No", user.Contact_No);
                insertCommand.Parameters.AddWithValue("@Password", user.Password);
                insertCommand.Parameters.AddWithValue("@Role_Id", user.Role_Id);

                var result = insertCommand.ExecuteNonQuery();
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
