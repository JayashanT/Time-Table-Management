using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
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
        public string key = "1234567890-abcde";

        public UserServices()
        {
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
                return "Staff Id already available";
            }
            string InsertCommand = "INSERT INTO Users (Name,Staff_Id,Contact_No,Password,Role_Id) VALUES(@Name,@Staff_Id,@Contact_No,@Password,@Role_Id)";
            try
            {
                SqlCommand insertCommand = new SqlCommand(InsertCommand, _dBContext.MainConnection);
                insertCommand.Parameters.AddWithValue("@Name", user.Name);
                insertCommand.Parameters.AddWithValue("@Staff_Id", user.Staff_Id);
                insertCommand.Parameters.AddWithValue("@Contact_No", user.Contact_No);
                insertCommand.Parameters.AddWithValue("@Password", password);
                insertCommand.Parameters.AddWithValue("@Role_Id", user.Role_Id);

                var result = insertCommand.ExecuteNonQuery();
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
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
                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

    }
}
