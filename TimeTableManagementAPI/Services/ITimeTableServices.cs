using TimeTableManagementAPI.Models;

namespace TimeTableManagementAPI.Services
{
    public interface ITimeTableServices
    {
        bool Add(Time_Table timeTable);
        string CreateAPeriodSlot(Slot slot);
        bool Update(Time_Table time_Table);
    }
}