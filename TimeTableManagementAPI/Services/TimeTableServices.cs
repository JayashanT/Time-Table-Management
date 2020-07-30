using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TimeTableAPI.Models;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.Repository;
using TimeTableManagementAPI.VM;

namespace TimeTableManagementAPI.Services
{
    public class TimeTableServices : ITimeTableServices
    {
        private ICommonRepository<Time_Table> _timetableRepo;
        private ICommonRepository<Slot> _slotRepo;
        private ICommonRepository<Users> _userRepo;
        string ConnectionInformation = "Server=localhost;Database=TimeTableDB;Trusted_Connection=True;MultipleActiveResultSets=true";
        //string ConnectionInformation = "Server=DESKTOP-QUN35J5\\Bhashitha;Database=TimeTableManagement123;Trusted_Connection=True;MultipleActiveResultSets=true";
        public TimeTableServices(ICommonRepository<Time_Table> timetableRepo, ICommonRepository<Slot> slotRepo, ICommonRepository<Users> userRepo)
        {
            _timetableRepo = timetableRepo;
            _slotRepo = slotRepo;
            _userRepo = userRepo;
        }

        public object Add(Time_Table timeTable)
        {
            using(SqlConnection Connection=new SqlConnection(ConnectionInformation))
            {
                Connection.Open();
                string checkTimeTableAvaiblity = "SELECT * from Time_Table where Class_Id=@Class_Id";
                SqlCommand checkCommand = new SqlCommand(checkTimeTableAvaiblity, Connection);
                checkCommand.Parameters.AddWithValue("@Class_Id", timeTable.Class_Id);

                SqlDataReader reader = checkCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Close();
                    return ("Time Table already Available For Class");
                }
                reader.Close();

                try
                {
                    using (SqlCommand insertCommand = new SqlCommand("INSERT INTO Time_Table(Name, Grade, Admin_Id, Class_Id) output inserted.Id VALUES(@Name, @Grade, @Admin_Id, @Class_Id)",Connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Name", timeTable.Name);
                        insertCommand.Parameters.AddWithValue("@Grade", timeTable.Grade);
                        insertCommand.Parameters.AddWithValue("@Admin_Id", timeTable.Admin_Id);
                        insertCommand.Parameters.AddWithValue("@Class_Id", timeTable.Class_Id);

                        int Id = (int)insertCommand.ExecuteScalar();
                        if (Id > 0)
                        {
                            Time_Table time_Table = new Time_Table()
                            {
                                Id = Id,
                                Name = timeTable.Name,
                                Grade = timeTable.Grade,
                                Admin_Id = timeTable.Grade,
                                Class_Id = timeTable.Class_Id
                            };
                            return (time_Table);
                        }
                        else
                        {
                            return false;
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
                finally
                {
                    Connection.Close();
                }
            }
           
        }

        public object Update(Time_Table time_Table)
        {
            using (SqlConnection Connection = new SqlConnection(ConnectionInformation))
            {
                Connection.Open();
                string InsertCommand = "UPDATE Time_Table SET Name=@Name,Grade=@Grade,Admin_Id=@Admin_Id,Class_Id=@Class_Id WHERE Id=" + time_Table.Id;
                try
                {
                    using (SqlCommand insertCommand = new SqlCommand(InsertCommand, Connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Name", time_Table.Name);
                        insertCommand.Parameters.AddWithValue("@Grade", time_Table.Grade);
                        insertCommand.Parameters.AddWithValue("@Admin_Id", time_Table.Admin_Id);
                        insertCommand.Parameters.AddWithValue("@Class_Id", time_Table.Class_Id);

                        int Id = (int)insertCommand.ExecuteScalar();
                        if (Id > 0)
                        {
                            Time_Table timeTable = new Time_Table()
                            {
                                Id = Id,
                                Name = time_Table.Name,
                                Grade = time_Table.Grade,
                                Admin_Id = time_Table.Grade,
                                Class_Id = time_Table.Class_Id
                            };
                            return (timeTable);
                        }
                        else
                            return "Time Table Update Not done";
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
                finally
                {
                    Connection.Close();
                }
            }
               
        }

        public object CreateAPeriodSlot(Slot slot)
        {
            using (SqlConnection Connection = new SqlConnection(ConnectionInformation))
            {
                Connection.Open();
                string checkSlot = "select * from slot where Time_Table_Id=@Time_Table_Id AND Period_No=@Period_No1";
                SqlCommand checkSlotCommand = new SqlCommand(checkSlot,Connection);
                checkSlotCommand.Parameters.AddWithValue("@Time_Table_Id", slot.Time_Table_Id);
                checkSlotCommand.Parameters.AddWithValue("@Period_No1", slot.Period_No);

                SqlDataReader checkSlotReader = checkSlotCommand.ExecuteReader();

                if (checkSlotReader.HasRows)
                {
                    checkSlotReader.Close();
                    return "Slot Already Allocated";
                }
                checkSlotReader.Close();

                string InsertCommand = "INSERT INTO Slot (Day,Start_Time,End_Time,Period_No,Time_Table_Id,Resource_Id,Teacher_Id,Subject_Id) " +
                    "output INSERTED.Id VALUES(@Day,@Start_Time,@End_Time,@Period_No,@Time_Table_Id,@Resource_Id,@Teacher_Id,@Subject_Id)";
                try
                {
                    using (SqlCommand insertCommand = new SqlCommand(InsertCommand,Connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Day", slot.Day);
                        insertCommand.Parameters.AddWithValue("@Start_Time", slot.Start_Time);
                        insertCommand.Parameters.AddWithValue("@End_Time", slot.End_Time);
                        insertCommand.Parameters.AddWithValue("@Period_No", slot.Period_No);
                        insertCommand.Parameters.AddWithValue("@Time_Table_Id", slot.Time_Table_Id);
                        insertCommand.Parameters.AddWithValue("@Resource_Id", slot.Resource_Id);
                        insertCommand.Parameters.AddWithValue("@Teacher_Id", slot.Teacher_Id);
                        insertCommand.Parameters.AddWithValue("@Subject_Id", slot.Subject_Id);

                        int Id = (int)insertCommand.ExecuteScalar();
                        if (Id > 0)
                        {
                            Slot Outslot = new Slot()
                            {
                                Id = Id,
                                Day = slot.Day,
                                Start_Time = slot.Start_Time,
                                End_Time = slot.End_Time,
                                Period_No = slot.Period_No,
                                Time_Table_Id = slot.Time_Table_Id,
                                Resource_Id = slot.Resource_Id,
                                Teacher_Id = slot.Teacher_Id,
                                Subject_Id = slot.Subject_Id
                            };
                            return (Outslot);
                        }
                        else
                            return "Error in Slot Creation";
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return "Error in Saving";
                }
                finally
                {
                    Connection.Close();
                }
            }
               
        }

        public object UpdatePeriodSlot(Slot slot)
        {
            using (SqlConnection Connection = new SqlConnection(ConnectionInformation))
            {
                try
                {
                Connection.Open();
                string checkSlot = "select * from slot where Id=@Id";
                SqlCommand checkSlotCommand = new SqlCommand(checkSlot, Connection);
                checkSlotCommand.Parameters.AddWithValue("@Id", slot.Id);

                SqlDataReader checkSlotReader = checkSlotCommand.ExecuteReader();

                if (!checkSlotReader.HasRows)
                {
                    checkSlotReader.Close();
                    return "No Slot Available";
                }
                checkSlotReader.Close();

                string InsertCommand = "Update Slot SET Day=@Day,Start_Time=@Start_Time,End_Time=@End_Time,Period_No=@Period_No,Time_Table_Id=@Time_Table_Id,Resource_Id=@Resource_Id,Teacher_Id=@Teacher_Id,Subject_Id=@Subject_Id WHERE Id=@Id";
                
                    using (SqlCommand insertCommand = new SqlCommand(InsertCommand,Connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Day", slot.Day);
                        insertCommand.Parameters.AddWithValue("@Start_Time", slot.Start_Time);
                        insertCommand.Parameters.AddWithValue("@End_Time", slot.End_Time);
                        insertCommand.Parameters.AddWithValue("@Period_No", slot.Period_No);
                        insertCommand.Parameters.AddWithValue("@Time_Table_Id", slot.Time_Table_Id);
                        insertCommand.Parameters.AddWithValue("@Resource_Id", slot.Resource_Id);
                        insertCommand.Parameters.AddWithValue("@Teacher_Id", slot.Teacher_Id);
                        insertCommand.Parameters.AddWithValue("@Subject_Id", slot.Subject_Id);
                        insertCommand.Parameters.AddWithValue("@Id", slot.Id);

                        var Id = insertCommand.ExecuteNonQuery();
                        if (Id > 0)
                        {
                            return (slot);
                        }
                        else
                            return "Error in Slot Creation";
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return "Error in Saving";
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
                   "left join Teacher_Subject t on u.Id = t.Teacher_Id WHERE t.Subject_Id = @Subject_Id ";
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
                    if (!result.Any())
                        return ("No Teachers available");
                    else
                        return result;
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

        public Object GetTimeTableDetails(int Id)
        {
            using (SqlConnection Connection = new SqlConnection(ConnectionInformation))
            {
                try
                {
                    Connection.Open();
                    var TableDetails = _timetableRepo.GetById("Time_Table", Id);
                    if (TableDetails == null)
                        return ("Time Table Not Found");
                    var AllSlotsOFATimeTable = _slotRepo.GetByOneParameter("Slot", "Time_Table_Id", Convert.ToString(Id));

                    string query = "SELECT distinct s.Id,s.Day,s.Period_No,s.Time_Table_Id,s.Resource_Id,s.Teacher_Id,u.Name AS Teacher_Name,s.Subject_Id,sb.Name AS Subject_Name" +
                        " FROM Slot s INNER JOIN Subject sb ON s.Subject_Id=sb.Id INNER JOIN users u ON s.Teacher_Id=u.Id WHERE s.Time_Table_Id=@S_Id";
                    using (SqlCommand QueryCommand = new SqlCommand(query, Connection))
                    {
                        QueryCommand.Parameters.AddWithValue("@S_Id", Id);
                        SqlDataReader reader = QueryCommand.ExecuteReader();
                        List<SlotVM> slotVMs = new List<SlotVM>().ToList();
                        while (reader.Read())
                        {
                            SlotVM slot = new SlotVM()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Day = reader["Day"].ToString(),
                                Period_No = reader["Period_No"].ToString(),
                                Time_Table_Id = Convert.ToInt32(reader["Time_Table_Id"]),
                                Teacher_Id = Convert.ToInt32(reader["Teacher_Id"]),
                                Teacher_Name = reader["Teacher_Name"].ToString(),
                                Subject_Id = Convert.ToInt32(reader["Subject_Id"]),
                                Subject_Name = reader["Subject_Name"].ToString()
                            };
                            if (reader["Resource_Id"] == DBNull.Value)
                                slot.Resource_Id = 0;
                            else
                                slot.Resource_Id = Convert.ToInt32(reader["Resource_Id"]);

                            slotVMs.Add(slot);
                        }

                        TableData td = new TableData()
                        {
                            Id = TableDetails.Id,
                            Name = TableDetails.Name,
                            Grade = TableDetails.Grade,
                            Class_id = TableDetails.Class_Id,
                            Admin_Id = TableDetails.Admin_Id,
                            slot = slotVMs

                        };
                        reader.Close();
                        return td;
                    }
                }catch(Exception e)
                {
                    return e.Message;
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        public object GetDetailsOfATimeTableByClassId(int ClassId)
        {
            using (SqlConnection Connection = new SqlConnection(ConnectionInformation))
            {
                try
                {
                    Connection.Open();
                    string TimeTableDetails = "select * from Time_Table where Class_Id=@ClassId ";
                    SqlCommand QueryCommand = new SqlCommand(TimeTableDetails, Connection);
                    QueryCommand.Parameters.AddWithValue("@ClassId", ClassId);

                    SqlDataReader reader = QueryCommand.ExecuteReader();
                    reader.Read();
                    if (!reader.HasRows)
                    {
                        reader.Close();
                        return ("Time Table Not Found");
                    }

                    Time_Table time_Table = new Time_Table()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = Convert.ToString(reader["Name"]),
                        Grade = Convert.ToInt32(reader["Grade"]),
                        Admin_Id = Convert.ToInt32(reader["Admin_Id"]),
                        Class_Id = ClassId
                    };
                    reader.Close();

                    string checkSlot = "select Id from Time_Table where Class_Id=@Class_Id";
                    SqlCommand checkSlotCommand = new SqlCommand(checkSlot, Connection);
                    checkSlotCommand.Parameters.AddWithValue("@Class_Id", ClassId);

                    SqlDataReader checkSlotReader = checkSlotCommand.ExecuteReader();
                    checkSlotReader.Read();

                    string query = "SELECT distinct s.Id,s.Day,s.Period_No,s.Time_Table_Id,s.Resource_Id,s.Teacher_Id,u.Name AS Teacher_Name,s.Subject_Id,sb.Name AS Subject_Name" +
                        " FROM Slot s INNER JOIN Subject sb ON s.Subject_Id=sb.Id INNER JOIN users u ON s.Teacher_Id=u.Id WHERE s.Time_Table_Id=@S_Id";
                    using (SqlCommand QueryCMD = new SqlCommand(query, Connection))
                    {
                        QueryCMD.Parameters.AddWithValue("@S_Id", Convert.ToInt32(checkSlotReader["Id"]));
                        checkSlotReader.Close();
                        SqlDataReader sreader = QueryCMD.ExecuteReader();
                        List<SlotVM> slotVMs = new List<SlotVM>().ToList();
                        while (sreader.Read())
                        {
                            SlotVM slot = new SlotVM()
                            {
                                Id = Convert.ToInt32(sreader["Id"]),
                                Day = sreader["Day"].ToString(),
                                Period_No = sreader["Period_No"].ToString(),
                                Time_Table_Id = Convert.ToInt32(sreader["Time_Table_Id"]),
                                Teacher_Id = Convert.ToInt32(sreader["Teacher_Id"]),
                                Teacher_Name = sreader["Teacher_Name"].ToString(),
                                Subject_Id = Convert.ToInt32(sreader["Subject_Id"]),
                                Subject_Name = sreader["Subject_Name"].ToString()
                            };
                            if (sreader["Resource_Id"] == DBNull.Value)
                                slot.Resource_Id = 0;
                            else
                                slot.Resource_Id = Convert.ToInt32(sreader["Resource_Id"]);

                            slotVMs.Add(slot);
                        }

                        TableData td = new TableData()
                        {
                            Id = time_Table.Id,
                            Name = time_Table.Name,
                            Grade = time_Table.Grade,
                            Class_id = time_Table.Class_Id,
                            Admin_Id = time_Table.Admin_Id,
                            slot = slotVMs

                        };
                        sreader.Close();
                        return td;

                    }
                }catch(Exception e)
                {
                    return e.Message;
                }
                finally
                {
                    Connection.Close();
                }
            }
               
        }

        public object AllSlotsOfATeacher(int Id)
        {
            using (SqlConnection Connection = new SqlConnection(ConnectionInformation))
            {
                try
                {
                    Connection.Open();
                    var IsTeacherAvailable = _userRepo.GetById("Users", Id);
                    if (IsTeacherAvailable == null)
                        return "Teacher Not Available";

                    string query = "SELECT DISTINCT S.*, sb.Name as Subject_Name,T.Class_Id, C.Name AS Class_Name " +
                        "from Slot S  INNER JOIN Time_Table T ON S.Time_Table_Id=T.Id INNER JOIN Subject sb ON S.Subject_Id=sb.Id INNER JOIN Class C ON T.Class_Id = C.Id" +
                        " WHERE S.Teacher_Id = @Teacher_Id";
                    SqlCommand queryCMD = new SqlCommand(query, Connection);
                    queryCMD.Parameters.AddWithValue("Teacher_Id", Id);

                    SqlDataReader reader = queryCMD.ExecuteReader();
                    List<SlotVM> slotVMs = new List<SlotVM>().ToList();
                    while (reader.Read())
                    {
                        SlotVM slot = new SlotVM()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Day = reader["Day"].ToString(),
                            Period_No = reader["Period_No"].ToString(),
                            Time_Table_Id = Convert.ToInt32(reader["Time_Table_Id"]),
                            Teacher_Id = Convert.ToInt32(reader["Teacher_Id"]),
                            Subject_Id = Convert.ToInt32(reader["Subject_Id"]),
                            Subject_Name = reader["Subject_Name"].ToString(),
                            Class_Id = Convert.ToInt32(reader["Class_Id"]),
                            Class_Name = reader["Class_Name"].ToString()
                        };
                        if (reader["Resource_Id"] == DBNull.Value)
                            slot.Resource_Id = 0;
                        else
                            slot.Resource_Id = Convert.ToInt32(reader["Resource_Id"]);

                        slotVMs.Add(slot);
                    }
                    return slotVMs;
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

    }
}
