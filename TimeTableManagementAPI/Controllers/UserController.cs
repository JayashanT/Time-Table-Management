using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TimeTableAPI.Models;
using TimeTableManagementAPI.Repository;
using TimeTableManagementAPI.Services;

namespace TimeTableAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController:ControllerBase
    {
        private IUserServices _userServices;
        private ICommonRepository<Users> _userRepository;
        public UserController(IUserServices userServices,ICommonRepository<Users> userRepository)
        {
            _userServices = userServices;
            _userRepository = userRepository;
        }

        public IActionResult GetAllUsers()
        {
            var Result = _userRepository.GetAll("Users");
            return Ok(Result);
        }

        [Route("{id}")]
        public IActionResult GetAUserById(int Id)
        {
            var Result = _userRepository.GetById("Users",Id);
            return Ok(Result);
        }

        [HttpPost]
        public IActionResult Add([FromBody]Users user)
        {
            var result = _userServices.Add(user);
            if (result)
                return Ok(result);
            else
                return BadRequest(result);

        }

        [HttpPut]
        public IActionResult Update([FromBody]Users user)
        {
            var result = _userServices.UpdateUser(user);
            if (result)
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}
