using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.Utility;
using System.Data.SqlClient;

namespace TimeTableManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController:Controller
    {
            private IConfiguration _config;
            DBContext _dBContext;

        public LoginController(IConfiguration config)
            {
                _config = config;
            _dBContext = new DBContext();
            }

            [AllowAnonymous]
            [HttpPost]
            public IActionResult Login([FromBody]LoginVM login)
            {
                IActionResult response = Unauthorized();
                var user = AuthenticateUser(login);

                if (user != null)
                {
                    var tokenString = GenerateJSONWebToken(user);
                    response = Ok(new { token = tokenString });
                }

                return response;
            }

            private string GenerateJSONWebToken(LoginVM userInfo)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                  _config["Jwt:Issuer"],
                  null,
                  expires: DateTime.Now.AddMinutes(120),
                  signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            private LoginVM AuthenticateUser(LoginVM login)
            {
                LoginVM user = null;

                string queryCommand = "Select * from Users where Staff_Id=@Staff_Id";
                SqlCommand myCommand = new SqlCommand(queryCommand, _dBContext.MainConnection);

                myCommand.Parameters.AddWithValue("@Staff_Id", login.Staff_Id);

                SqlDataReader reader = myCommand.ExecuteReader();

            //Validate the User Credentials    
            //Demo Purpose, I have Passed HardCoded User Information    
            //if (login.Username == "Jignesh")
            //{
            //   user = new LoginVM { Username = "Jignesh Trivedi", EmailAddress = "test.btest@gmail.com" };
            //}
            return user;
            }
    }
}
