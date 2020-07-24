using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TimeTableAPI.Models;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.Repository;
using TimeTableManagementAPI.Utility;

namespace TimeTableManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController:Controller
    {
        private ICommonRepository<Attendance> _attendanceRepository;
        private ICommonRepository<Users> _userRepository;
        string ConnectionInformation = "Server=localhost;Database=TimeTableDB;Trusted_Connection=True;MultipleActiveResultSets=true";

        public AttendanceController(ICommonRepository<Attendance> attendanceRepository, ICommonRepository<Users> userRepository)
        {
            _attendanceRepository = attendanceRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        public IActionResult SetAttendanceForADay()
        {
            var AllUsers = _userRepository.GetAll("Users");

            using(SqlConnection Connection=new SqlConnection(ConnectionInformation))
            {
                Connection.Open();
                foreach (var user in AllUsers)
                {
                    string SetAttendance = "INSERT INTO Attendance (Date, Status, User_id) VALUES(@Date,@Status,@User_Id)";
                    SqlCommand SetAttendanceCMD = new SqlCommand(SetAttendance, Connection);
                    SetAttendanceCMD.Parameters.AddWithValue("@Date", System.DateTime.Today);
                    SetAttendanceCMD.Parameters.AddWithValue("@Status", Convert.ToByte(false));
                    SetAttendanceCMD.Parameters.AddWithValue("@User_id", user.Id);

                    var Result = SetAttendanceCMD.ExecuteScalar();
                };
                Connection.Close();
                return Ok("All attendance set to false");
            }    
        }

        [HttpPut]
        public IActionResult MarkAttendance([FromBody]Attendance attendance)
        {
            using (SqlConnection Connection = new SqlConnection(ConnectionInformation))
            {
                try
                {
                    Connection.Open();
                    string SetAttendance = "UPDATE Attendance SET Status=@Status WHERE User_id=@User_Id AND Date=@Date";
                    SqlCommand SetAttendanceCMD = new SqlCommand(SetAttendance,Connection);
                    SetAttendanceCMD.Parameters.AddWithValue("@Date", attendance.Date);
                    SetAttendanceCMD.Parameters.AddWithValue("@Status", attendance.Status);
                    SetAttendanceCMD.Parameters.AddWithValue("@User_id", attendance.User_Id);

                    var Result = SetAttendanceCMD.ExecuteNonQuery();
                    Connection.Close();
                    if (Result > 0)
                        return Ok(attendance);
                    else
                        return BadRequest("Something went wrong");
                }
                catch (Exception e)
                {
                    Connection.Close();
                    return BadRequest("Something went wrong");
                }
            }
               
        }

        public IActionResult GetAllAtendees(DateTime Date)
        {
            using (SqlConnection Connection = new SqlConnection(ConnectionInformation))
            {
                Connection.Open();
                string attendQueryString = "select * from Attendance where Status=1 AND Date=@Date";
                SqlCommand QueryCommand = new SqlCommand(attendQueryString, Connection);
                QueryCommand.Parameters.AddWithValue("@Date", Date);

                SqlDataReader reader = QueryCommand.ExecuteReader();

                List<Attendance> attendances = new List<Attendance>();
                while (reader.Read())
                {
                    Attendance attendance = new Attendance()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        User_Id = Convert.ToInt32(reader["User_Id"])
                    };
                    attendances.Add(attendance);
                }
                Connection.Close();
                if (attendances.Any())
                    return Ok(attendances);
                else
                    return BadRequest("Not Found any attendace");

            }
        }
    }
}
