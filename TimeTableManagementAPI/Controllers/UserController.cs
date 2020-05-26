using AutoMapper;
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
    public class UserController:Controller
    {
        private IUserServices _userServices;
        private ICommonRepository _userRepository;
        public UserController(IUserServices userServices,ICommonRepository userRepository)
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

    }
}
