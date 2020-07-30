using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.VM;

namespace TimeTableManagementAPI.Services
{
    public class ReliefServices : IReliefServices
    {
        string ConnectionInformation = "Server=localhost;Database=TimeTableDB;Trusted_Connection=True;MultipleActiveResultSets=true";
        //string ConnectionInformation = "Server=DESKTOP-QUN35J5\\Bhashitha;Database=TimeTableManagement123;Trusted_Connection=True;MultipleActiveResultSets=true";
        public ReliefServices()
        {
        }

        public object FindVanantSlotsInADay()
        {
            using(SqlConnection Connection=new SqlConnection(ConnectionInformation))
            {
                try
                {
                    var day = DateTime.Today.DayOfWeek.ToString();
                    Connection.Open();
                    string Query = "SELECT distinct S.*,SB.Name as Subject_Name " +
                        "FROM SLOT S INNER JOIN ATTENDANCE A ON S.Teacher_Id=A.User_Id INNER JOIN Subject SB on SB.Id=S.Subject_Id " +
                        "WHERE S.Day LIKE @Day and A.Status = 0 and a.DATE = CONVERT(date, GETDATE()) ";
                    SqlCommand QueryCMD = new SqlCommand(Query, Connection);
                    QueryCMD.Parameters.AddWithValue("@Date", DateTime.Today);
                    QueryCMD.Parameters.AddWithValue("@Day", "Monday");

                    SqlDataReader reader = QueryCMD.ExecuteReader();
                    List<SlotVM> slots = new List<SlotVM>();
                    while (reader.Read())
                    {
                        SlotVM slot = new SlotVM()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Day = Convert.ToString(reader["Day"]),
                            //Start_Time = reader.GetTimeSpan(2),
                            //End_Time = reader.GetTimeSpan(3),
                            Period_No = Convert.ToString(reader["Period_No"]),
                            Time_Table_Id = Convert.ToInt32(reader["Time_Table_Id"]),
                            //Resource_Id = Convert.ToInt32(reader["Resource_Id"]),
                            Teacher_Id = Convert.ToInt32(reader["Teacher_Id"]),
                            Subject_Id = Convert.ToInt32(reader["Subject_Id"]),
                            Subject_Name = reader["Subject_Name"].ToString(),
                        };
                        slots.Add(slot);
                    }
                    reader.Close();

                    string getAllocatePeriods = "SELECT Slot_Id from Updates where Date= CONVERT(date, GETDATE())";
                    SqlCommand command = new SqlCommand(getAllocatePeriods, Connection);
                    SqlDataReader SlotReader = command.ExecuteReader();
                    while (SlotReader.Read())
                    {
                        var result=slots.Find(x=>x.Id==Convert.ToInt32(SlotReader["Slot_Id"]));
                        //var result = slots.Find(x => x.Id == 6);
                        slots.Remove(result);
                    }

                    SlotReader.Close();
                    if (slots.Any())
                        return (slots);
                    else
                        return "No slots not covered";
                }
                catch(Exception e)
                {
                    return e.Message;
                }
                finally
                {
                    Connection.Close();
                }
                
            }
            
        }

        public object GetAllTeachersAvailableForSlotForASubject(string PeriodNo, int SubjectId)
        {
            using (SqlConnection Connection = new SqlConnection(ConnectionInformation))
            {
                try
                {
                    Connection.Open();
                    string AvailablityTeachers = "select distinct u.Id,u.Name,s.Period_No from users u left join slot s on u.Id = s.Teacher_Id " +
                   "left join Teacher_Subject t on u.Id = t.Teacher_Id INNER JOIN Attendance A ON s.Teacher_Id=A.User_Id " +
                   "WHERE t.Subject_Id = @Subject_Id AND A.DATE = CONVERT(date, GETDATE()) AND A.Status=1";

                    SqlCommand QueryCommand = new SqlCommand(AvailablityTeachers, Connection);
                    QueryCommand.Parameters.AddWithValue("@Subject_Id", SubjectId);

                    SqlDataReader reader = QueryCommand.ExecuteReader();
                    List<AvailableTeachers> entities = new List<AvailableTeachers>();
                    while (reader.Read())
                    {
                        AvailableTeachers user = new AvailableTeachers()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = Convert.ToString(reader["Name"]),
                            Period_No = Convert.ToString(reader["Period_No"]),
                        };
                        entities.Add(user);
                    }
                    foreach (var entry in entities.ToList())
                    {
                        if (entry.Period_No == PeriodNo)
                        {
                            foreach (var user in entities.ToList())
                                if (user.Id == entry.Id)
                                    entities.Remove(user);

                        }
                    }
                    var result = entities.GroupBy(X => X.Id).Select(x => x.First());
                    reader.Close();
                    Connection.Close();
                    if (!result.Any())
                        return ("No Teachers available");
                    else
                        return result;
                }
                catch (Exception e)
                {
                    return e.Message.ToString();
                }
                finally
                {
                    Connection.Close();
                }
            }    
        }

        public object AddAReliefUpdate(Updates update)
        {
            using(SqlConnection Connection=new SqlConnection(ConnectionInformation))
            {
                try
                {
                    Connection.Open();
                    string query = "INSERT INTO Updates (Date,Status,Admin_Id,Teacher_Id,Slot_Id) output INSERTED.Id VALUES(@Date,@Status,@Admin_Id,@Teacher_Id,@Slot_Id)";
                    SqlCommand insertCMD = new SqlCommand(query, Connection);
                    insertCMD.Parameters.AddWithValue("@Date", DateTime.Today);
                    insertCMD.Parameters.AddWithValue("@Status", 0);
                    insertCMD.Parameters.AddWithValue("@Admin_Id", update.Admin_Id);
                    insertCMD.Parameters.AddWithValue("@Teacher_Id", update.Teacher_Id);
                    insertCMD.Parameters.AddWithValue("@Slot_Id", update.Slot_Id);

                    int Id = (int)insertCMD.ExecuteScalar();
                    if (Id > 0)
                    {
                        update.Id = Id;
                        return update;
                    }
                    else
                        return "Relief update not saved";
                }
                catch (Exception e)
                {
                    return e.Message;
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        public object ApproveAreliefRequest(int Id)
        {
            using(SqlConnection Connection=new SqlConnection(ConnectionInformation))
            {
                try
                {
                    Connection.Open();
                    string query = "Update Updates SET Status=@Status WHERE Id=@Id";
                    SqlCommand updateCMD = new SqlCommand(query, Connection);
                    updateCMD.Parameters.AddWithValue("@Id", Id);
                    updateCMD.Parameters.AddWithValue("@Status", 1);

                    var Result = updateCMD.ExecuteNonQuery();
                    if (Result > 0)
                        return "Relief Updated";
                    else
                        return "Fail to Update Relief";
                }
                catch (Exception e)
                {
                    return e.Message;
                }
                finally
                {
                    Connection.Close();
                }
            }
           
        }

        public object UpdateARelief(Updates update)
        {
            using(SqlConnection Connection=new SqlConnection(ConnectionInformation))
            {
                try
                {
                    Connection.Open();
                    string query = "Update Updates set Date=@Date,Status=@Status,Admin_Id=@Admin_Id,Teacher_Id=@Teacher_Id,Slot_Id=@Slot_Id WHERE Id=@Id";
                    SqlCommand updateCMD = new SqlCommand(query, Connection);
                    updateCMD.Parameters.AddWithValue("@Id", update.Id);
                    updateCMD.Parameters.AddWithValue("@Date", update.Date);
                    updateCMD.Parameters.AddWithValue("@Status", update.Status);
                    updateCMD.Parameters.AddWithValue("@Admin_Id", update.Admin_Id);
                    updateCMD.Parameters.AddWithValue("@Teacher_Id", update.Teacher_Id);
                    updateCMD.Parameters.AddWithValue("@Slot_Id", update.Slot_Id);

                    var Id = updateCMD.ExecuteNonQuery();

                    updateCMD.Connection.Close();
                    if (Id > 0)
                    {
                        return update;
                    }
                    else
                        return "Relief update not saved";
                }
                catch (Exception e)
                {
                    return e.Message;
                }
                finally
                {
                    Connection.Close();
                }
            }  
        }

        public object GetAllReliefRequests()
        {
            using(SqlConnection Connection = new SqlConnection(ConnectionInformation))
            {
                try
                {
                    string query = "SELECT * FROM updates Where Date=CONVERT(date, GETDATE())";
                    SqlCommand command = new SqlCommand(query, Connection);

                    var Reader = command.ExecuteReader();
                    if (Reader.HasRows)
                    {
                        List<Updates> updates = new List<Updates>();
                        while (Reader.Read())
                        {
                            Updates update = new Updates()
                            {
                                Id = Convert.ToInt32(Reader["Id"]),
                                Status = Convert.ToByte(Reader["Status"]),
                                Admin_Id = Convert.ToInt32(Reader["Admin_Id"]),
                                Slot_Id = Convert.ToInt32(Reader["Slot_Id"]),
                                Teacher_Id = Convert.ToInt32(Reader["Teacher_Id"]),
                            };
                        }
                        Reader.Close();
                        return updates;
                    }
                    else
                        return "No Allocations for Today";
                   
                }
                catch (Exception e)
                {
                    return e.Message.ToString();
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        public object GetAllReleifAllocationsByTeacherId(int userId)
        {
            using (SqlConnection Connection = new SqlConnection(ConnectionInformation))
            {
                try
                {
                    string query = "SELECT * FROM updates Where Date=CONVERT(date, GETDATE()) AND User_Id=@userId";
                    SqlCommand command = new SqlCommand(query, Connection);
                    command.Parameters.AddWithValue("@userId", userId);
                    var Reader = command.ExecuteReader();
                    if (Reader.HasRows)
                    {
                        List<Updates> updates = new List<Updates>();
                        while (Reader.Read())
                        {
                            Updates update = new Updates()
                            {
                                Id = Convert.ToInt32(Reader["Id"]),
                                Status = Convert.ToByte(Reader["Status"]),
                                Admin_Id = Convert.ToInt32(Reader["Admin_Id"]),
                                Slot_Id = Convert.ToInt32(Reader["Slot_Id"]),
                                Teacher_Id = Convert.ToInt32(Reader["Teacher_Id"]),
                            };
                        }
                        Reader.Close();
                        return updates;
                    }
                    else
                        return "No Allocations for Today";

                }
                catch (Exception e)
                {
                    return e.Message.ToString();
                }
                finally
                {
                    Connection.Close();
                }
            }
        }
    }
}