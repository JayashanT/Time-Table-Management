using System.Collections.Generic;

namespace TimeTableManagementAPI.Repository
{
    public interface ICommonRepository<TEntity> where TEntity : class
    {
        bool DeleteRecord(string table, int Id);
        IEnumerable<TEntity> GetAll(string table);
        IEnumerable<TEntity> GetById(string table, int Id);
        IEnumerable<TEntity> GetByOneParameter(string table, string Name, string value);
    }
}