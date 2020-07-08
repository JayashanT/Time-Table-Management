using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableManagementAPI.Models
{
    public class LoginVM
    {
        public int Id { get; set; }
        public int Staff_Id { get; set; }
        public string Password { get; set; }
    }
}
