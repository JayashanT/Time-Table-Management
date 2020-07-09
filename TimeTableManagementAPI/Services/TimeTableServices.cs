using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.Utility;

namespace TimeTableManagementAPI.Services
{
    public class TimeTableServices
    {
        DBContext _dBContext;
        public TimeTableServices()
        {
            _dBContext = new DBContext();
        }

        public bool Add(Time_Table timeTable)
        {
            string InsertCommand = "INSERT INTO Users (Name,Grade,Admin_Id) VALUES(@Name,@Grade,@Admin_Id)";
            try
            {
                SqlCommand insertCommand = new SqlCommand(InsertCommand, _dBContext.MainConnection);
                insertCommand.Parameters.AddWithValue("@Name", timeTable.Name);
                insertCommand.Parameters.AddWithValue("@Grade", timeTable.Grade);
                insertCommand.Parameters.AddWithValue("@Admin_Id", timeTable.Admin_Id);

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

        public bool Update(Time_Table time_Table)
        {
                string InsertCommand = "UPDATE Time_Table SET Name=@Name,Grade=@Grade,Admin_Id=@Admin_Id WHERE Id=" + time_Table.Id;
                try
                {
                    SqlCommand insertCommand = new SqlCommand(InsertCommand, _dBContext.MainConnection);
                    insertCommand.Parameters.AddWithValue("@Name", time_Table.Name);
                    insertCommand.Parameters.AddWithValue("@Grade", time_Table.Grade);
                    insertCommand.Parameters.AddWithValue("@Admin_Id", time_Table.Admin_Id );

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

        //public 
    }    
}
