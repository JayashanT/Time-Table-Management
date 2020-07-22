using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableManagementAPI.VM
{
    public class SlotVM
    {
        public int Id { get; set; }
        public string Day { get; set; }
        public TimeSpan Start_Time { get; set; }
        public TimeSpan End_Time { get; set; }
        public string Period_No { get; set; }
        public int Time_Table_Id { get; set; }
        public int Resource_Id { get; set; }
        public int Teacher_Id { get; set; }
        public int Subject_Id { get; set; }
        public string Teacher_Name { get; set; }
        public string Subject_Name { get; set; }
        public int Class_Id { get; set; }
        public string Class_Name { get; set; }
    }
}
