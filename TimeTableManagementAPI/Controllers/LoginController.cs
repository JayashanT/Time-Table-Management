using System;
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
using TimeTableManagementAPI.Services;
using TimeTableAPI.Models;

namespace TimeTableManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController:Controller
    {
            private IConfiguration _config;
            IUserServices _userServices;
            DBContext _dBContext;

        public LoginController(IConfiguration config,IUserServices userServices)
        {
            _config = config;
            _dBContext = new DBContext();
            _userServices = userServices;
        }

            [AllowAnonymous]
            [HttpPost]
            public IActionResult Login([FromBody]Users login)
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

            private string GenerateJSONWebToken(Users userInfo)
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

            private Users AuthenticateUser(Users login)
            {
                Users user =new Users();

                string queryCommand = "Select * from Users where Staff_Id=@Staff_Id";
                SqlCommand myCommand = new SqlCommand(queryCommand, _dBContext.MainConnection);

                myCommand.Parameters.AddWithValue("@Staff_Id", login.Staff_Id);

                SqlDataReader reader = myCommand.ExecuteReader();
                UserServices us = new UserServices();

            if (reader.HasRows)
            {
                reader.Read();
                String Password = _userServices.Decrypt(Convert.ToString(reader["Password"]),us.key);
                Console.WriteLine(Password);
                if (Password == login.Password)
                {
                    user.Id = Convert.ToInt32(reader["Id"]);
                    user.Staff_Id = Convert.ToString(reader["Staff_Id"]);
                    user.Name = Convert.ToString(reader["Name"]);
                    user.Contact_No = Convert.ToString(reader["Contact_No"]);
                }
                else return null;

                return user;
            }
            else return null;
        }
            
    }
}
