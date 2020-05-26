using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableManagementAPI.Repository
{
    //public class CommonRepository<TEntity> : ICommonRepository where TEntity : class
    public class CommonRepository : ICommonRepository
    {
        string ConnectionInformation = "Server=localhost;Database=TimeTableDB;Trusted_Connection=True;MultipleActiveResultSets=true";
        SqlConnection MainConnection;
        public CommonRepository()
        {
            MainConnection = new SqlConnection(ConnectionInformation);
            MainConnection.Open();
        }

        public DataSet GetAll(string table)
        {
            try
            {
                string MyCommand = "Select * from " + table;
                SqlCommand myCommand = new SqlCommand(MyCommand, MainConnection);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);
                DataSet ds = new DataSet();
                da.Fill(ds);

                return ds;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            
        }

        public DataSet GetById(string table,int Id)
        {
            try {
                string MyCommand = "Select * from " + table + " where Id=" + Id;
                SqlCommand myCommand = new SqlCommand(MyCommand, MainConnection);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public bool DeleteRecord(string table,int Id)
        {
            string MyCommand = "Delete * from " + table + " where Id=" + Id;
            SqlCommand myCommand = new SqlCommand(MyCommand, MainConnection);
            try
            {
                var Result=myCommand.ExecuteNonQuery();
                if (Result != 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
