using System.Collections.Generic;
using TimeTableAPI.Models;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.VM;

namespace TimeTableManagementAPI.Services
{
    public interface ITimeTableServices
    {
        bool Add(Time_Table timeTable);
        string CreateAPeriodSlot(Slot slot);
        bool Update(Time_Table time_Table);
        IEnumerable<AvailableTeachers> GetAllTeachersAvailableForSlotForASubject(int PeriodNo, string Day, int SubjectId);
    }
}