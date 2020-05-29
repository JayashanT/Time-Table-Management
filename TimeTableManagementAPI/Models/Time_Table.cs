using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableManagementAPI.Models
{
    public class Time_Table
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Grade { get; set; }
        public int Admin_Id { get; set; }
    }
}
