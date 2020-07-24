using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TimeTableAPI.Models;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.Repository;

namespace TimeTableManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController:Controller
    {
        private ICommonRepository<Subject> _subjectRepository;
        private ICommonRepository<Teacher_Subject> _subjectTeacherRepository;
        private ICommonRepository<Users> _userRepository;
        string ConnectionInformation = "Server=localhost;Database=TimeTableDB;Trusted_Connection=True;MultipleActiveResultSets=true";
        public SubjectController(ICommonRepository<Subject> subjectRepository, ICommonRepository<Teacher_Subject> subjectTeacherRepository, 
            ICommonRepository<Users> userRepository)
        {
            _subjectRepository = subjectRepository;
            _subjectTeacherRepository = subjectTeacherRepository;
            _userRepository = userRepository;
        }

        public IActionResult GetAllSubjects()
        {
            var result = _subjectRepository.GetAll("Subject");
            if (result != null)
                return Ok(result);
            else
                return BadRequest("No subjects Found");
        }

        [Route("{id}")]
        public IActionResult GetASubjectById(int Id)
        {
            var result = _subjectRepository.GetById("Subject", Id);
            if (result != null)
                return Ok(result);
            else
                return BadRequest("No subject details Found");
        }

        [HttpDelete]
        public IActionResult DeleteSubject(int Id)
        {
            var Result = _subjectRepository.DeleteRecord("Subject", Id);
            if (Result)
                return Ok("Subject Deleted");
            else
                return BadRequest("Something went wrong");
        }

        [HttpPost]
        public IActionResult Add([FromBody]Subject subject)
        {
            using(SqlConnection Connection=new SqlConnection(ConnectionInformation))
            {
                string InsertCommand = "INSERT INTO Subject (Name,Medium) VALUES(@Name,@Medium)";
                try
                {
                    Connection.Open();
                    SqlCommand insertCommand = new SqlCommand(InsertCommand, Connection);
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
                finally
                {
                    Connection.Close();
                }
            }
        }

        [HttpPut]
        public IActionResult UpdareSubject([FromBody]Subject subject)
        {
            using(SqlConnection Connection=new SqlConnection(ConnectionInformation))
            {
                string InsertCommand = "Update Subject set Name=@Name,Medium=@Medium where Id=@Id";
                try
                {
                    Connection.Open();
                    SqlCommand updateCMD = new SqlCommand(InsertCommand, Connection);
                    updateCMD.Parameters.AddWithValue("@Name", subject.Name);
                    updateCMD.Parameters.AddWithValue("@Medium", subject.Medium);
                    updateCMD.Parameters.AddWithValue("@Id", subject.Id);

                    var result = updateCMD.ExecuteNonQuery();
                    if (result > 0)
                        return Ok(subject);
                    else
                        return BadRequest("Update failed");
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

        [Route("GetAllTeachersForASubject/{id}")]
        public IActionResult GetAllTeachersForASubject(int Id)
        {
            using(SqlConnection Connection=new SqlConnection(ConnectionInformation))
            {
                try
                {
                    Connection.Open();
                    DataTable dt = new DataTable();
                    string MyCommand = "Select u.Id,u.Name from users u INNER JOIN Teacher_Subject t on t.Teacher_Id=u.Id where t.Subject_Id=@Subject_Id";
                    SqlCommand myCommand = new SqlCommand(MyCommand, Connection);
                    myCommand.Parameters.AddWithValue("@Subject_Id", Id);
                    SqlDataAdapter da = new SqlDataAdapter(myCommand);
                    da.Fill(dt);

                    List<Users> entities = new List<Users>(dt.Rows.Count);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow record in dt.Rows)
                        {
                            Users item = _userRepository.GetItem<Users>(record);
                            entities.Add(item);
                        }
                    }
                    return Ok(entities);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
                finally
                {
                    Connection.Close();
                }
            }
    }

        [Route("GetAllSubjectsOfATeacher/{id}")]
        public IActionResult GetAllSubjectsOfATeacher(int Id)
        {
            var result = _subjectTeacherRepository.GetByOneParameter("Teacher_Subject", "Teacher_Id", Convert.ToString(Id));
            if (result != null)
                return Ok(result);
            else
                return BadRequest("No subjects Found for teacher");
        }

        [HttpPost]
        [Route("teacher_subject")]
        public IActionResult AssignASubjectToATeacher([FromBody]Teacher_Subject teacher_Subject)
        {
            using(SqlConnection Connection=new SqlConnection(ConnectionInformation))
            {
                string InsertCommand = "INSERT INTO Teacher_Subject (Teacher_Id,Subject_Id) VALUES(@Teacher_Id,@Subject_Id)";
                try
                {
                    Connection.Open();
                    SqlCommand insertCommand = new SqlCommand(InsertCommand, Connection);
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
                finally
                {
                    Connection.Close();
                }
            }
           
            
        }

        [HttpDelete]
        [Route("RemoveSubjectFromTeacher/{id}")]
        public IActionResult RemoveAllocatedSubjectsFromTeachers(int Id)
        {
            var Result = _subjectTeacherRepository.DeleteRecord("Teacher_Subject",Id);
            if (Result)
                return Ok("Subject Deleted");
            else
                return BadRequest("Something went wrong");
        }
    }
}
