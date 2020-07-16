using System;
using System.Collections.Generic;
using Microsoft;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using TimeTableManagementAPI.Utility;
using System.Data;

namespace TimeTableManagementAPI.Repository
{
    public class CommonRepository<TEntity> : ICommonRepository<TEntity> where TEntity :class
    {

        DBContext _dBContext;
        public CommonRepository()
        {
            _dBContext = new DBContext();
        }

        public IEnumerable<TEntity> GetAll(string table)
        {
            try
             {
                DataTable dt = new DataTable();
                string MyCommand = "Select * from " + table;
                SqlCommand myCommand = new SqlCommand(MyCommand, _dBContext.MainConnection);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);
                //DataSet ds = new DataSet();
                da.Fill(dt);

                List<TEntity> entities = new List<TEntity>(dt.Rows.Count);
                if (dt.Rows.Count > 0)
                {
                    foreach(DataRow record in dt.Rows)
                    {
                        TEntity item = GetItem<TEntity>(record);

                        entities.Add(item);
                        //entities.Add(new Read(record));
                    }
                    
                }

                return entities;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            
        }

        public IEnumerable<TEntity> GetById(string table,int Id)
        {
            try {
                DataTable dt = new DataTable();
                string MyCommand = "Select * from " + table + " where Id=" + Id;
                SqlCommand myCommand = new SqlCommand(MyCommand, _dBContext.MainConnection);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);
                da.Fill(dt);

                List<TEntity> entities = new List<TEntity>(dt.Rows.Count);
                if (dt.Rows.Count > 0)
                {
                    TEntity item = GetItem<TEntity>(dt.Rows[0]);
                    entities.Add(item);
                }
                else return null;

                return entities;


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public IEnumerable<TEntity> GetByOneParameter(string table, string Name, string value)
        {
            try
            {
                DataTable dt = new DataTable();
                string MyCommand = "Select * from " + table + " where "+Name+"=" + value;
                SqlCommand myCommand = new SqlCommand(MyCommand, _dBContext.MainConnection);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);
                da.Fill(dt);

                List<TEntity> entities = new List<TEntity>(dt.Rows.Count);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow record in dt.Rows)
                    {
                        TEntity item = GetItem<TEntity>(record);
                        entities.Add(item);
                    }

                }

                return entities;


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
            SqlCommand myCommand = new SqlCommand(MyCommand, _dBContext.MainConnection);
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

        public TEntity GetItem<T>(DataRow dr)
        {
            Type temp = typeof(TEntity);
            TEntity obj = Activator.CreateInstance<TEntity>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        if(dr[column.ColumnName]== DBNull.Value)
                            {
                                pro.SetValue(obj, null, null);
                            }
                        else
                            pro.SetValue(obj, dr[column.ColumnName], null);
                    } 
                    else
                        continue;
                }
            }
            return obj;
        }
    }
}
