using TimeTableManagementAPI.Models;

namespace TimeTableManagementAPI.Services
{
    public interface IReliefServices
    {
        object AddAReliefUpdate(Updates update);
        object ApproveAreliefRequest(int Id);
        object FindVanantSlotsInADay();
        object UpdateARelief(Updates update);
    }
}