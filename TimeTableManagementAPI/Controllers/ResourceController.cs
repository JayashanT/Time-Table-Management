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
    public class ResourceController : Controller
    {
        DBContext _dBContext;
        ICommonRepository<Resource> _resourceRepo;
        public ResourceController(ICommonRepository<Resource> resourceRepo)
        {
            _dBContext = new DBContext();
            _resourceRepo = resourceRepo;
        }

        [HttpPost]
        public IActionResult AddResource([FromBody]Resource resource)
        {
            string InsertCommand = "INSERT INTO Resource (Name,Type) output INSERTED.Id VALUES(@Name,@Type)";
            try
            {
                using (SqlCommand insertCommand = new SqlCommand(InsertCommand, _dBContext.MainConnection))
                {
                    insertCommand.Parameters.AddWithValue("@Name", resource.Name);
                    insertCommand.Parameters.AddWithValue("@Type", resource.Type);

                    int result = (int)insertCommand.ExecuteScalar();
                    _dBContext.MainConnection.Close();
                    if (result > 0)
                    {
                        resource.Id = result;
                        return Ok(resource);
                    }
                    else
                        return BadRequest("Resource add failed");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _dBContext.MainConnection.Close();
                return BadRequest();
            }
        }

        public IActionResult UpdateResource([FromBody]Resource resource)
        {
            string InsertCommand = "UPDATE Resource SET Name=@Name,Type=@Type WHERE Id=@Id";
            try
            {
                using (SqlCommand insertCommand = new SqlCommand(InsertCommand, _dBContext.MainConnection))
                {
                    insertCommand.Parameters.AddWithValue("@Name", resource.Name);
                    insertCommand.Parameters.AddWithValue("@Type", resource.Type);
                    insertCommand.Parameters.AddWithValue("@Id", resource.Id);

                    var result = insertCommand.ExecuteNonQuery();
                    _dBContext.MainConnection.Close();
                    if (result > 0)
                    {
                        return Ok(resource);
                    }
                    else
                        return BadRequest("Update failed");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _dBContext.MainConnection.Close();
                return BadRequest();
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

        public IActionResult GetAllAvailbleResourcesForASlot(int periodNo)
        {
            string AvailableResources = "SELECT distinct R.ID,R.NAME,R.Type FROM Resource R LEFT JOIN SLOT S ON R.ID=S.Resource_Id WHERE S.Period_No!=@Period_No";
            SqlCommand QueryCommand = new SqlCommand(AvailableResources, _dBContext.MainConnection);
            QueryCommand.Parameters.AddWithValue("@Period_No", periodNo);
            SqlDataReader reader = QueryCommand.ExecuteReader();

            List<Resource> resources = new List<Resource>();
            while (reader.Read())
            {
                Resource resource = new Resource()
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = Convert.ToString(reader["Name"]),
                    Type= Convert.ToString(reader["Type"])
                };
                resources.Add(resource);
            }
            _dBContext.MainConnection.Close();
            if (!resources.Any())
                return Ok("No Resources Available");
            else
                return Ok(resources);
        }
    }
}