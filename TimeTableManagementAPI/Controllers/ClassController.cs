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
        public ClassController(ICommonRepository<Class> classRepository)
        {
            _classRepository = classRepository;
            _dBContext = new DBContext();
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