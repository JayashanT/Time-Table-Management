using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTableManagementAPI.Models;

namespace TimeTableManagementAPI.VM
{
    public class TableData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Grade { get; set; }
        public int Admin_Id { get; set; }
        public List<Slot> slot { get; set; }
    }
}
