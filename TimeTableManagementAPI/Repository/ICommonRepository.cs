using System.Collections.Generic;
using System.Data;

namespace TimeTableManagementAPI.Repository
{
    public interface ICommonRepository<TEntity> where TEntity : class
    {
        bool DeleteRecord(string table, int Id);
        IEnumerable<TEntity> GetAll(string table);
        TEntity GetById(string table, int Id);
        IEnumerable<TEntity> GetByOneParameter(string table, string Name, string value);
        TEntity GetItem<T>(DataRow dr);
    }
}