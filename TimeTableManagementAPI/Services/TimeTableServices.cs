using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TimeTableAPI.Models;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.Repository;
using TimeTableManagementAPI.Utility;
using TimeTableManagementAPI.VM;

namespace TimeTableManagementAPI.Services
{
    public class TimeTableServices : ITimeTableServices
    {
        DBContext _dBContext;
        ICommonRepository<Time_Table> _timetableRepo;
        ICommonRepository<Slot> _slotRepo;
        public TimeTableServices(ICommonRepository<Time_Table> timetableRepo,ICommonRepository<Slot> slotRepo)
        {
            _dBContext = new DBContext();
            _timetableRepo = timetableRepo;
            _slotRepo = slotRepo;
        }

        public object Add(Time_Table timeTable)
        {
            string checkTimeTableAvaiblity = "SELECT * from Time_Table where Class_Id=@Class_Id";
            SqlCommand checkCommand = new SqlCommand(checkTimeTableAvaiblity, _dBContext.MainConnection);
            checkCommand.Parameters.AddWithValue("@Class_Id", timeTable.Class_Id);

            SqlDataReader reader = checkCommand.ExecuteReader();
            if (reader.HasRows)
            {
                return ("Time Table already Available For Class");
            }

           // string InsertCommand = "INSERT INTO Time_Table (Name,Grade,Admin_Id,Class_Id) output INSERTED.Id VALUES(@Name,@Grade,@Admin_Id,@Class_Id)";
            try
            {
                using (SqlCommand insertCommand = new SqlCommand("INSERT INTO Time_Table(Name, Grade, Admin_Id, Class_Id) output inserted.Id VALUES(@Name, @Grade, @Admin_Id, @Class_Id)", _dBContext.MainConnection))
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
                            Id=Id,
                            Name=timeTable.Name,
                            Grade=timeTable.Grade,
                            Admin_Id=timeTable.Grade,
                            Class_Id=timeTable.Class_Id
                        };
                        return (time_Table);
                    }
                    else
                        return false;
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public object Update(Time_Table time_Table)
        {
            string InsertCommand = "UPDATE Time_Table SET Name=@Name,Grade=@Grade,Admin_Id=@Admin_Id,Class_Id=@Class_Id WHERE Id=" + time_Table.Id;
            try
            {
                using (SqlCommand insertCommand = new SqlCommand(InsertCommand, _dBContext.MainConnection))
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
        }

        public object CreateAPeriodSlot(Slot slot)
        {
            
            string checkSlot = "select * from slot where Time_Table_Id=@Time_Table_Id AND Period_No=@Period_No1";
            SqlCommand checkSlotCommand = new SqlCommand(checkSlot, _dBContext.MainConnection);
            checkSlotCommand.Parameters.AddWithValue("@Time_Table_Id", slot.Time_Table_Id);
            checkSlotCommand.Parameters.AddWithValue("@Period_No1", slot.Period_No);

            SqlDataReader checkSlotReader = checkSlotCommand.ExecuteReader();

            if (checkSlotReader.HasRows)
            {
                return "Slot Already Allocated";
            }
            checkSlotReader.Close();

            string InsertCommand = "INSERT INTO Slot (Day,Start_Time,End_Time,Period_No,Time_Table_Id,Resource_Id,Teacher_Id,Subject_Id) " +
                "output INSERTED.Id VALUES(@Day,@Start_Time,@End_Time,@Period_No,@Time_Table_Id,@Resource_Id,@Teacher_Id,@Subject_Id)";
            try
            {
                using (SqlCommand insertCommand = new SqlCommand(InsertCommand, _dBContext.MainConnection))
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
                            Id=Id,
                            Day=slot.Day,
                            Start_Time=slot.Start_Time,
                            End_Time=slot.End_Time,
                            Period_No=slot.Period_No,
                            Time_Table_Id=slot.Time_Table_Id,
                            Resource_Id=slot.Resource_Id,
                            Teacher_Id=slot.Teacher_Id,
                            Subject_Id=slot.Subject_Id
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
        }

        public IEnumerable<AvailableTeachers> GetAllTeachersAvailableForSlotForASubject(string PeriodNo, int SubjectId)
        {
            DataTable dt = new DataTable();
            string AvailablityTeachers = "select distinct u.Id,u.Name,s.Period_No from users u left join slot s on u.Id = s.Teacher_Id " +
                "left join Teacher_Subject t on u.Id = t.Teacher_Id WHERE t.Subject_Id = @Subject_Id ";
            SqlCommand QueryCommand = new SqlCommand(AvailablityTeachers, _dBContext.MainConnection);
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
                        if(user.Id==entry.Id)
                            entities.Remove(user);

                }
            }
            var result=entities.GroupBy(X => X.Id).Select(x=>x.First());
            return result;
        }

        public Object GetTimeTableDetails(int Id)
        {
            var TableDetails=_timetableRepo.GetById("Time_Table",Id);
            var AllSlotsOFATimeTable = _slotRepo.GetByOneParameter("Slot", "Time_Table_Id", Convert.ToString(Id));
            return new
            {
                TableDetails,
                AllSlotsOFATimeTable
            };

        }
    }    
}
