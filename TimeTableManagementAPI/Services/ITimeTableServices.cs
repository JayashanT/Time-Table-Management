using System.Collections.Generic;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.VM;

namespace TimeTableManagementAPI.Services
{
    public interface ITimeTableServices
    {
        bool Add(Time_Table timeTable);
        string CreateAPeriodSlot(Slot slot);
        IEnumerable<AvailableTeachers> GetAllTeachersAvailableForSlotForASubject(string PeriodNo, int SubjectId);
        bool Update(Time_Table time_Table);
    }
}