using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.Utility;

namespace TimeTableManagementAPI.Services
{
    public class ReliefServices : IReliefServices
    {
        private DBContext _dBContext;
        public ReliefServices()
        {
            _dBContext = new DBContext();
        }

        public object FindVanantSlotsInADay()
        {
            string Query = "SELECT * FROM SLOT S INNER JOIN ATTENDANCE A ON S.Teacher_Id=A.User_Id WHERE A.Date=@Date AND A.Status=0";
            SqlCommand QueryCMD = new SqlCommand(Query, _dBContext.MainConnection);
            QueryCMD.Parameters.AddWithValue("@Date", DateTime.Today);

            SqlDataReader reader = QueryCMD.ExecuteReader();
            List<Slot> slots = new List<Slot>();
            while (reader.Read())
            {
                Slot slot = new Slot()
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Day = Convert.ToString(reader["Day"]),
                    //Start_Time = reader.GetTimeSpan(2),
                    //End_Time = reader.GetTimeSpan(3),
                    Period_No = Convert.ToString(reader["Period_No"]),
                    Time_Table_Id = Convert.ToInt32(reader["Time_Table_Id"]),
                    Resource_Id = Convert.ToInt32(reader["Resource_Id"]),
                    Teacher_Id = Convert.ToInt32(reader["Teacher_Id"]),
                    Subject_Id = Convert.ToInt32(reader["Subject_Id"]),
                };
                slots.Add(slot);
            }
            reader.Close();
            QueryCMD.Connection.Close();
            _dBContext.MainConnection.Close();
            if (slots.Any())
                return (slots);
            else
                return "No slots not covered";
        }

        public object AddAReliefUpdate(Updates update)
        {
            try
            {
                string query = "INSERT INTO Updates (Date,Status,Admin_Id,Teacher_Id,Slot_Id) values output INSERTED.Id VALUES(@Date,@Status,@Admin_Id,@Teacher_Id,@Slot_Id)";
                SqlCommand insertCMD = new SqlCommand(query, _dBContext.MainConnection);
                insertCMD.Parameters.AddWithValue("@Date", DateTime.Today);
                insertCMD.Parameters.AddWithValue("@Status", 0);
                insertCMD.Parameters.AddWithValue("@Admin_Id", update.Admin_Id);
                insertCMD.Parameters.AddWithValue("@Teacher_Id", update.Teacher_Id);
                insertCMD.Parameters.AddWithValue("@Slot_Id", update.Slot_Id);

                int Id = (int)insertCMD.ExecuteScalar();
                insertCMD.Connection.Close();
                _dBContext.MainConnection.Close();
                if (Id > 0)
                {
                    update.Id = Id;
                    return update;
                }
                else
                    return "Relief update not saved";
            }
            catch(Exception e)
            {
                _dBContext.MainConnection.Close();
                return e.Message;
            }
        }

        public object ApproveAreliefRequest(int Id)
        {
            try
            {
                string query = "Update Updates SET Status=@Status WHERE Id=@Id";
                SqlCommand updateCMD = new SqlCommand(query, _dBContext.MainConnection);
                updateCMD.Parameters.AddWithValue("@Id", Id);
                updateCMD.Parameters.AddWithValue("@Status", 1);

                var Result = updateCMD.ExecuteNonQuery();
                updateCMD.Connection.Close();
                _dBContext.MainConnection.Close();
                if (Result > 0)
                    return "Relief Updated";
                else
                    return "Fail to Update Relief";
            }catch(Exception e)
            {
                _dBContext.MainConnection.Close();
                return e.Message;
            }  
        }

        public object UpdateARelief(Updates update)
        {
            try
            {
                string query = "Update Updates set Date=@Date,Status=@Status,Admin_Id=@Admin_Id,Teacher_Id=@Teacher_Id,Slot_Id=@Slot_Id WHERE Id=@Id";
                SqlCommand updateCMD = new SqlCommand(query, _dBContext.MainConnection);
                updateCMD.Parameters.AddWithValue("@Id", update.Id);
                updateCMD.Parameters.AddWithValue("@Date", update.Date);
                updateCMD.Parameters.AddWithValue("@Status", update.Status);
                updateCMD.Parameters.AddWithValue("@Admin_Id", update.Admin_Id);
                updateCMD.Parameters.AddWithValue("@Teacher_Id", update.Teacher_Id);
                updateCMD.Parameters.AddWithValue("@Slot_Id", update.Slot_Id);

                var Id = updateCMD.ExecuteNonQuery();

                updateCMD.Connection.Close();
                _dBContext.MainConnection.Close();
                if (Id > 0)
                {
                    return update;
                }
                else
                    return "Relief update not saved";
            }
            catch (Exception e)
            {
                _dBContext.MainConnection.Close();
                return e.Message;
            }
        }
    }
}