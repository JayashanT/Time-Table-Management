using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TimeTableAPI.Models;
using TimeTableManagementAPI.Utility;

namespace TimeTableManagementAPI.Services
{
    public class UserServices : IUserServices
    {
        DBContext _dBContext;
        private IConfiguration _config;
        public string key = "1234567890-abcde";

        public UserServices(IConfiguration config)
        {
            _config = config;
            _dBContext = new DBContext();
        }

        public object Add(Users user)
        {
            var password = Encrypt(user.Password, key);
            string checkStaffId = "Select Staff_Id from Users where Staff_Id=@Staff_Id";
            SqlCommand StaffIdCommand = new SqlCommand(checkStaffId, _dBContext.MainConnection);
            StaffIdCommand.Parameters.AddWithValue("@Staff_Id", user.Staff_Id);

            SqlDataReader reader = StaffIdCommand.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Close();
                return "Staff Id already available";
            }
            reader.Close();
            string InsertCommand = "INSERT INTO Users (Name,Staff_Id,Contact_No,Password,Role_Id) output INSERTED.Id VALUES(@Name,@Staff_Id,@Contact_No,@Password,@Role_Id)";
            try
            {
                SqlCommand insertCommand = new SqlCommand(InsertCommand, _dBContext.MainConnection);
                insertCommand.Parameters.AddWithValue("@Name", user.Name);
                insertCommand.Parameters.AddWithValue("@Staff_Id", user.Staff_Id);
                insertCommand.Parameters.AddWithValue("@Contact_No", user.Contact_No);
                insertCommand.Parameters.AddWithValue("@Password", password);
                insertCommand.Parameters.AddWithValue("@Role_Id", user.Role_Id);

                int Id = (int)insertCommand.ExecuteScalar();
                if (Id > 0)
                {
                    Users ReturnUser = new Users()
                    {
                        Id=Id,
                        Name=user.Name,
                        Staff_Id=user.Staff_Id,
                        Contact_No=user.Contact_No,
                        Role_Id=user.Role_Id
                    };
                    var tokenString = GenerateJSONWebToken(ReturnUser);
                    return (new { token = tokenString });
                }
                else
                    return "Someting Went wrong";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Someting Went wrong";
            }
        }

        public string Encrypt(string password, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(password);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public string Decrypt(string password, string keyString)
        {
            var fullCipher = Convert.FromBase64String(password);

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }



        public object UpdateUser(Users user)
        {
            string InsertCommand = "UPDATE Users SET Name=@Name,Staff_Id=@Staff_Id,Contact_No=@Contact_No,Password=@Password,Role_Id=@Role_Id WHERE Id=" + user.Id;
            try
            {
                SqlCommand insertCommand = new SqlCommand(InsertCommand, _dBContext.MainConnection);
                insertCommand.Parameters.AddWithValue("@Name", user.Name);
                insertCommand.Parameters.AddWithValue("@Staff_Id", user.Staff_Id);
                insertCommand.Parameters.AddWithValue("@Contact_No", user.Contact_No);
                insertCommand.Parameters.AddWithValue("@Password", user.Password);
                insertCommand.Parameters.AddWithValue("@Role_Id", user.Role_Id);

                var result = insertCommand.ExecuteNonQuery();
                _dBContext.MainConnection.Close();
                if (result > 0)
                {
                    Users ReturnUser = new Users()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Staff_Id = user.Staff_Id,
                        Contact_No = user.Contact_No,
                        Role_Id = user.Role_Id
                    };
                    var tokenString = GenerateJSONWebToken(ReturnUser);
                    return (new { token = tokenString });
                }
                else
                {
                    _dBContext.MainConnection.Close();
                    return "Update Failed";
                }
                    
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public string GenerateJSONWebToken(Users userInfo)
        {
            var tokenHandeler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id",userInfo.Id.ToString()),
                    new Claim("Staff_Id",userInfo.Staff_Id.ToString()),
                    new Claim("Name",userInfo.Name),
                    new Claim("Role_Id",userInfo.Role_Id.ToString()),
                    new Claim("Contact_No",userInfo.Contact_No.ToString()),

                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandeler.CreateToken(tokenDescriptor);
            return tokenHandeler.WriteToken(token);
        }

        public Users AuthenticateUser(Users login)
        {
            Users user = new Users();

            string queryCommand = "Select * from Users where Staff_Id=@Staff_Id";
            SqlCommand myCommand = new SqlCommand(queryCommand, _dBContext.MainConnection);

            myCommand.Parameters.AddWithValue("@Staff_Id", login.Staff_Id);

            SqlDataReader reader = myCommand.ExecuteReader();
 

            if (reader.HasRows)
            {
                reader.Read();
                String Password = Decrypt(Convert.ToString(reader["Password"]), key);
                Console.WriteLine(Password);
                if (Password == login.Password)
                {
                    user.Id = Convert.ToInt32(reader["Id"]);
                    user.Staff_Id = Convert.ToString(reader["Staff_Id"]);
                    user.Name = Convert.ToString(reader["Name"]);
                    user.Contact_No = Convert.ToString(reader["Contact_No"]);
                    user.Role_Id = Convert.ToInt32(reader["Role_Id"]);
                    reader.Close();
                    _dBContext.MainConnection.Close();
                }
                else
                {
                    reader.Close();
                    _dBContext.MainConnection.Close();
                    return null;
                }
                return user;
            }
            else
            {
                reader.Close();
                _dBContext.MainConnection.Close();
                return null;
            }
                
        }

    }
}
