using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.Repository;

namespace TimeTableManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : Controller
    {
        string ConnectionInformation = "Server=localhost;Database=TimeTableDB;Trusted_Connection=True;MultipleActiveResultSets=true";
        //string ConnectionInformation = "Server=DESKTOP-QUN35J5\\Bhashitha;Database=TimeTableManagement123;Trusted_Connection=True;MultipleActiveResultSets=true";
        private ICommonRepository<Resource> _resourceRepo;
        public ResourceController(ICommonRepository<Resource> resourceRepo)
        {
            _resourceRepo = resourceRepo;
        }

        [HttpPost]
        public IActionResult AddResource([FromBody]Resource resource)
        {
            using (SqlConnection Connection = new SqlConnection(ConnectionInformation))
            {
                try
                {
                    Connection.Open();
                    string InsertCommand = "INSERT INTO Resource (Name,Type) output INSERTED.Id VALUES(@Name,@Type)";
                    SqlCommand insertCommand = new SqlCommand(InsertCommand, Connection);
                    insertCommand.Parameters.AddWithValue("@Name", resource.Name);
                    insertCommand.Parameters.AddWithValue("@Type", resource.Type);

                    int result = (int)insertCommand.ExecuteScalar();
                    if (result > 0)
                    {
                        resource.Id = result;
                        return Ok(resource);
                    }
                    else
                        return BadRequest("Resource add failed");
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
        [Route("UpdateResource")]
        public IActionResult UpdateResource([FromBody]Resource resource)
        {
            using(SqlConnection Connection=new SqlConnection(ConnectionInformation))
            {
                string InsertCommand = "UPDATE Resource SET Name=@Name,Type=@Type WHERE Id=@Id";
                try
                {
                    Connection.Open();
                    SqlCommand insertCommand = new SqlCommand(InsertCommand, Connection);
                    insertCommand.Parameters.AddWithValue("@Name", resource.Name);
                    insertCommand.Parameters.AddWithValue("@Type", resource.Type);
                    insertCommand.Parameters.AddWithValue("@Id", resource.Id);

                    var result = insertCommand.ExecuteNonQuery();
                    if (result > 0)
                    {
                        return Ok(resource);
                    }
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

        public IActionResult GetAllResources()
        {
            var result = _resourceRepo.GetAll("Resource");
            if (result != null)
                return Ok(result);
            else
                return BadRequest("No resources Found");
        }

        [Route("{id}")]
        public IActionResult GetResourceById(int Id)
        {
            var result = _resourceRepo.GetById("Resource", Id);
            if (result != null)
                return Ok(result);
            else
                return BadRequest("No resources Found");
        }

        [Route("GetAvailableResources")]
        public IActionResult GetAllAvailbleResourcesForASlot(string periodNo)
        {
            using(SqlConnection Connection = new SqlConnection(ConnectionInformation))
            {
                Connection.Open();
                string AvailableResources = "SELECT distinct R.ID,R.NAME,R.Type FROM Resource R LEFT JOIN SLOT S ON R.ID=S.Resource_Id WHERE S.Period_No!=@Period_No";
                SqlCommand QueryCommand = new SqlCommand(AvailableResources, Connection);
                QueryCommand.Parameters.AddWithValue("@Period_No", periodNo);
                SqlDataReader reader = QueryCommand.ExecuteReader();

                List<Resource> resources = new List<Resource>();
                while (reader.Read())
                {
                    Resource resource = new Resource()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = Convert.ToString(reader["Name"]),
                        Type = Convert.ToString(reader["Type"])
                    };
                    resources.Add(resource);
                }
                Connection.Close();
                if (!resources.Any())
                    return Ok("No Resources Available");
                else
                    return Ok(resources);
            }
           
        }
    }
}