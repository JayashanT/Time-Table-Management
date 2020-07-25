using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.Repository;

namespace TimeTableManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : Controller
    {
        private ICommonRepository<Class> _classRepository;
        private ICommonRepository<Time_Table> _timeTableRepo;
        private ICommonRepository<Slot> _slotRepo;
        string ConnectionInformation = "Server=localhost;Database=TimeTableDB;Trusted_Connection=True;MultipleActiveResultSets=true";
        //string ConnectionInformation = "Server=DESKTOP-QUN35J5\\Bhashitha;Database=TimeTableManagement123;Trusted_Connection=True;MultipleActiveResultSets=true";

        public ClassController(ICommonRepository<Class> classRepository, ICommonRepository<Time_Table> timeTableRepo, ICommonRepository<Slot> slotRepo)
        {
            _classRepository = classRepository;
            _timeTableRepo = timeTableRepo;
            _slotRepo = slotRepo;
        }

        public IActionResult GetAllClasses()
        {
            var Result = _classRepository.GetAll("Class");
            return Ok(Result);
        }

        [Route("{id}")]
        public IActionResult GetAClassById(int Id)
        {
            var Result = _classRepository.GetById("Class", Id);
            return Ok(Result);
        }

        [HttpPost]
        public IActionResult Add([FromBody]Class classData)
        {
            using (SqlConnection Connection =new SqlConnection(ConnectionInformation))
            {
                Connection.Open();
                string InsertCommand = "INSERT INTO Class (Name,Grade) VALUES(@Name,@Grade)";
                try
                {
                    SqlCommand insertCommand = new SqlCommand(InsertCommand, Connection);
                    insertCommand.Parameters.AddWithValue("@Name", classData.Name);
                    insertCommand.Parameters.AddWithValue("@Grade", classData.Grade);

                    var result = insertCommand.ExecuteNonQuery();
                    if (result > 0)
                        return Ok();
                    else
                        return BadRequest();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return BadRequest();
                }
                finally
                {
                    Connection.Close();
                }
            }
           

        }

        [HttpPut]
        public IActionResult Upadte([FromBody]Class classData)
        {
            using(SqlConnection Connection= new SqlConnection(ConnectionInformation))
            {
                string UpdateQuery = "UPDATE Class SET Name=@Name,Grade=@Grade where Id=@Id";
                try
                {
                    Connection.Open();
                    SqlCommand updateCMD = new SqlCommand(UpdateQuery, Connection);
                    updateCMD.Parameters.AddWithValue("@Id", classData.Id);
                    updateCMD.Parameters.AddWithValue("@Name", classData.Name);
                    updateCMD.Parameters.AddWithValue("@Grade", classData.Grade);

                    var result = updateCMD.ExecuteNonQuery();
                    if (result > 0)
                        return Ok(classData);
                    else
                        return BadRequest("Update Failed");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return BadRequest("Update Failed");
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        [HttpDelete]
        public IActionResult DeleteClass(int Id)
        {
            using(SqlConnection Connection= new SqlConnection(ConnectionInformation))
            {
                string query = "SELECT Id from Time_Table where Class_Id=@ClassId";
                try
                {
                    Connection.Open();
                    SqlCommand querryCMD = new SqlCommand(query, Connection);
                    querryCMD.Parameters.AddWithValue("@CLassId", Id);

                    int TimeTableId = 0;
                    var reader = querryCMD.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        TimeTableId = Convert.ToInt32(reader["Id"]);
                    }
                    reader.Close();
                    var allSLots = _slotRepo.GetByOneParameter("Slot", "Time_Table_Id", TimeTableId.ToString());
                    foreach (var slot in allSLots)
                    {
                        _slotRepo.DeleteRecord("Slot", slot.Id);
                    };

                    _timeTableRepo.DeleteRecord("Time_Table", TimeTableId);

                    var result = _classRepository.DeleteRecord("class", Id);
                    if (result)
                        return Ok("Class Successfully deleted");
                    else
                        return BadRequest("Record not deleted");
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
                finally
                {
                    Connection.Close();
                }
            }
        }


        [Route("GetClassesRelateToGrade/{grade}")]
        public IActionResult GetAllClassesOfAGrade(int grade)
        {
            var Result = _classRepository.GetByOneParameter("Class", "Grade", Convert.ToString(grade));
            if (Result!=null)
                return Ok(Result);
            else
                return BadRequest("No Classes found");
        }


    }
}