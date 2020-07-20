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
            if (Result != null)
                return Ok(Result);
            else
                return BadRequest("Slot not found");
        }

        [HttpPost]
        public IActionResult Add([FromBody]Users user)
        {
            var Result = _userServices.Add(user);
            if (Result.GetType() == typeof(string))
                return BadRequest(Result);
            else
                return Ok(Result);


        }

        [HttpPut]
        public IActionResult Update([FromBody]Users user)
        {
            var Result = _userServices.Add(user);
            if (Result.GetType() == typeof(string))
                return Ok(Result);
            else
                return BadRequest(Result);
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody]Users login)
        {
            IActionResult response = Unauthorized();
            var user = _userServices.AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = _userServices.GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }
            else
                response = BadRequest("Password or Staff id incorrect");
            return response;
        }
    }
}
