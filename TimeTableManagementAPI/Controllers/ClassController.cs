using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.Repository;
using TimeTableManagementAPI.Utility;

namespace TimeTableManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : Controller
    {
        private ICommonRepository<Class> _classRepository;
        private DBContext _dBContext;
        private ICommonRepository<Time_Table> _timeTableRepo;
        private ICommonRepository<Slot> _slotRepo;

        public ClassController(ICommonRepository<Class> classRepository, ICommonRepository<Time_Table> timeTableRepo, ICommonRepository<Slot> slotRepo)
        {
            _classRepository = classRepository;
            _timeTableRepo = timeTableRepo;
            _slotRepo = slotRepo;
            _dBContext = new DBContext();
        }

        public IActionResult GetAllClasses()
        {
            _dBContext.MainConnection.Close();
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
            string InsertCommand = "INSERT INTO Class (Name,Grade) VALUES(@Name,@Grade)";
            try
            {
                SqlCommand insertCommand = new SqlCommand(InsertCommand, _dBContext.MainConnection);
                insertCommand.Parameters.AddWithValue("@Name", classData.Name);
                insertCommand.Parameters.AddWithValue("@Grade", classData.Grade);

                var result = insertCommand.ExecuteNonQuery();
                _dBContext.MainConnection.Close();
                if (result > 0)
                    return Ok();
                else
                    return BadRequest();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _dBContext.MainConnection.Close();
                return BadRequest();
            }
            finally
            {
                _dBContext.MainConnection.Close();
            }

        }

        [HttpPut]
        public IActionResult Upadte([FromBody]Class classData)
        {
            string UpdateQuery = "UPDATE Class SET Name=@Name,Grade=@Grade where Id=@Id";
            try
            {
                SqlCommand updateCMD = new SqlCommand(UpdateQuery, _dBContext.MainConnection);
                updateCMD.Parameters.AddWithValue("@Id", classData.Id);
                updateCMD.Parameters.AddWithValue("@Name", classData.Name);
                updateCMD.Parameters.AddWithValue("@Grade", classData.Grade);

                var result = updateCMD.ExecuteNonQuery();
                updateCMD.Connection.Close();
                if (result > 0)
                    return Ok(classData);
                else
                    return BadRequest("Update Failed");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Update Failed");
            }
            finally
            {
                _dBContext.MainConnection.Close();
            }
        }

        [HttpDelete]
        public IActionResult DeleteClass(int Id)
        {
            var timeTable = _classRepository.GetById("Class", Id);
            int timeTableId = timeTable.Id;

            var allSLots = _slotRepo.GetByOneParameter("Slot", "Time_Table_Id", timeTableId.ToString());
            foreach (var slot in allSLots)
            {
                _slotRepo.DeleteRecord("Slot", slot.Id);
            };

            _timeTableRepo.DeleteRecord("Time_Table", timeTableId);

            var result = _classRepository.DeleteRecord("class", Id);
            if (result)
                return Ok("Class Successfully deleted");
            else
                return BadRequest("Record not deleted");
        }


        [Route("GetClassesRelateToGrade/{grade}")]
        public IActionResult GetAllClassesOfAGrade(int grade)
        {
            _dBContext.MainConnection.Close();
            var Result = _classRepository.GetByOneParameter("Class", "Grade", Convert.ToString(grade));
            if (Result!=null)
                return Ok(Result);
            else
                return BadRequest("No Classes found");
        }


    }
}