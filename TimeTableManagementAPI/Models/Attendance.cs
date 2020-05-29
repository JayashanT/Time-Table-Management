using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableManagementAPI.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public DateTime MyProperty { get; set; }
        public Byte Status { get; set; }
        public int User_Id { get; set; }
    }
}
