using System;
using System.Collections.Generic;
using TimeTableManagementAPI.Models;
using TimeTableManagementAPI.VM;

namespace TimeTableManagementAPI.Services
{
    public interface ITimeTableServices
    {
        object Add(Time_Table timeTable);
        object CreateAPeriodSlot(Slot slot);
        IEnumerable<AvailableTeachers> GetAllTeachersAvailableForSlotForASubject(string PeriodNo, int SubjectId);
        object Update(Time_Table time_Table);
        Object GetTimeTableDetails(int Id);
        object GetDetailsOfATimeTableByClassId(int ClassId);
    }
}