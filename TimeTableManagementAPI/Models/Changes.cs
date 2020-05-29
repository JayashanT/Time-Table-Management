using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableManagementAPI.Models
{
    public class Changes
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string Status { get; set; }
        public int Admin_Id { get; set; }
        public int Teacher_Id { get; set; }
    }
}
