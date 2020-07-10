using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.Repository;
using TimeTableManagementAPI.Utility;

namespace TimeTableManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController:Controller
    {
        ICommonRepository<Subject> _subjectRepository;
        ICommonRepository<Teacher_Subject> _subjectTeacherRepository;
        DBContext _dBContext;
        public SubjectController(ICommonRepository<Subject> subjectRepository, ICommonRepository<Teacher_Subject> subjectTeacherRepository)
        {
            _subjectRepository = subjectRepository;
            _subjectTeacherRepository = subjectTeacherRepository;
            _dBContext = new DBContext();
        }

        public IActionResult GetAllSubjects()
        {
            var Result = _subjectRepository.GetAll("Subject");
            return Ok(Result);
        }

        [Route("{id}")]
        public IActionResult GetASubjectById(int Id)
        {
            var Result = _subjectRepository.GetById("Subject", Id);
            return Ok(Result);
        }

        [HttpPost]
        public IActionResult Add([FromBody]Subject subject)
        {
            string InsertCommand = "INSERT INTO Subject (Name,Medium) VALUES(@Name,@Medium)";
            try
            {
                SqlCommand insertCommand = new SqlCommand(InsertCommand, _dBContext.MainConnection);
                insertCommand.Parameters.AddWithValue("@Name", subject.Name);
                insertCommand.Parameters.AddWithValue("@Medium", subject.Medium);

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
        }

        [Route("GetAllTeachersForASubject")]
        public IActionResult GetAllTeachersForASubject(int Id)
        {
            //string value = Convert.ToString(Id);
            var result=_subjectTeacherRepository.GetByOneParameter("Teacher_Subject","Subject_Id", Convert.ToString(Id));
            return Ok(result);
        }

        [Route("GetAllSubjectsOfATeacher")]
        public IActionResult GetAllSubjectsOfATeacher(int Id)
        {
            var result = _subjectTeacherRepository.GetByOneParameter("Teacher_Subject", "Teacher_Id", Convert.ToString(Id));
            return Ok(result);
        }

        [HttpPost]
        [Route("teacher_subject")]
        public IActionResult AssignASubjectToATeacher([FromBody]Teacher_Subject teacher_Subject)
        {
            string InsertCommand = "INSERT INTO Teacher_Subject (Teacher_Id,Subject_Id) VALUES(@Teacher_Id,@Subject_Id)";
            try
            {
                SqlCommand insertCommand = new SqlCommand(InsertCommand, _dBContext.MainConnection);
                insertCommand.Parameters.AddWithValue("@Teacher_Id", teacher_Subject.Teacher_Id);
                insertCommand.Parameters.AddWithValue("@Subject_Id", teacher_Subject.Subject_Id);

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
        }
    }
}
