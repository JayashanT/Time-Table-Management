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
    public class ResourceController:Controller
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
            string InsertCommand = "INSERT INTO Resource (Name,Type) VALUES(@Name,@Type)";
            try
            {
                SqlCommand insertCommand = new SqlCommand(InsertCommand, _dBContext.MainConnection);
                insertCommand.Parameters.AddWithValue("@Name", resource.Name);
                insertCommand.Parameters.AddWithValue("@Type", resource.Type);

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

        public IActionResult UpdateResource([FromBody]Resource resource)
        {
            string InsertCommand = "UPDATE Users SET Name=@Name,Type=@Type WHERE Id=@Id";
            try
            {
                SqlCommand insertCommand = new SqlCommand(InsertCommand, _dBContext.MainConnection);
                insertCommand.Parameters.AddWithValue("@Name", resource.Name);
                insertCommand.Parameters.AddWithValue("@Type", resource.Type);
                insertCommand.Parameters.AddWithValue("@Id", resource.Id);

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

        public IActionResult GetAllResources()
        {
            var Result=_resourceRepo.GetAll("Resource");
            return Ok(Result);
        }

        [Route("{id}")]
        public IActionResult GetResourceById(int Id)
        {
            var Result = _resourceRepo.GetById("Resource",Id);
            return Ok(Result);
        }

    }
}
