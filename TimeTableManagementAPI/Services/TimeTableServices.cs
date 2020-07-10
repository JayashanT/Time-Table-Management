using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TimeTableAPI.Models;
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

        public string CreateAPeriodSlot(Slot slot)
        {
            //if teacher not availble for the slot
            string checkCommand = "select * from slot where Teacher_id=@Teacher_Id AND Day like @Day AND Period_No=@Period_No";
            SqlCommand CheckAvailablility = new SqlCommand(checkCommand, _dBContext.MainConnection);
            CheckAvailablility.Parameters.AddWithValue("@Teacher_Id", slot.Teacher_Id);
            CheckAvailablility.Parameters.AddWithValue("@Day", slot.Day);
            CheckAvailablility.Parameters.AddWithValue("@Period_No", slot.Period_No);

            SqlDataReader reader = CheckAvailablility.ExecuteReader();

            if (reader.HasRows)
            {
                return "Teacher is not available for this slot";
            }

            string InsertCommand = "INSERT INTO Users (Day,Stat_Time,End_Time,Period_No,Time_Table_Id,Resource_Id,Teacher_Id,Subject_Id) " +
                "VALUES(@Day,@Stat_Time,@End_Time,@Period_No,@Time_Table_Id,@Resource_Id,@Teacher_Id,@Subject_Id)";
            try
            {
                SqlCommand insertCommand = new SqlCommand(InsertCommand, _dBContext.MainConnection);
                insertCommand.Parameters.AddWithValue("@Day", slot.Day);
                insertCommand.Parameters.AddWithValue("@Stat_Time", slot.Start_Time);
                insertCommand.Parameters.AddWithValue("@End_Time", slot.End_Time);
                insertCommand.Parameters.AddWithValue("@Period_No", slot.Period_No);
                insertCommand.Parameters.AddWithValue("@Time_Table_Id", slot.Time_Table_Id);
                insertCommand.Parameters.AddWithValue("@Resource_Id", slot.Resource_Id);
                insertCommand.Parameters.AddWithValue("@Teacher_Id", slot.Teacher_Id);
                insertCommand.Parameters.AddWithValue("@Subject_Id", slot.Subject_Id);

                var result = insertCommand.ExecuteNonQuery();
                if (result > 0)
                    return "Slot Saved.";
                else
                    return "Error in Saving";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Error in Saving";
            }
        }
        /*
        public IEnumerable<Users> GetAllTeachersAvailableForSlotForASubject()
        {
            string AvailablityTeachers = "select * from users u inner join slot s on u.Id = s.Teacher_Id " +
                "WHERE S.Teacher_Id != @Teacher_Id AND u.Role_Id != 1 AND s.Period_No != @Period_No";
            SqlCommand QueryCommand = new SqlCommand(AvailablityTeachers, _dBContext.MainConnection);
            QueryCommand.Parameters.AddWithValue("@Teacher_Id", slot.Day);
            QueryCommand.Parameters.AddWithValue("@Period_No", slot.Day);

        }*/
    }    
}
