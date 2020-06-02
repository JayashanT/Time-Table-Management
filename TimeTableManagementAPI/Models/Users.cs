using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableAPI.Models
{
    public class Users
    {
        public string Name { get; set; }
        public string Staff_Id { get; set; }
        public string Contact_No { get; set; }
        public string Password { get; set; }
        public int Role_Id { get; set; }
    }
}
