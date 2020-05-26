using System.Data;

namespace TimeTableManagementAPI.Repository
{
    public interface ICommonRepository
    {
        DataSet GetAll(string table);
        DataSet GetById(string table, int Id);
    }
}