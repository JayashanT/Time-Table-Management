﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableManagementAPI.Utility
{
    public class DBContext
    {
        string ConnectionInformation = "Server=localhost;Database=TimeTableDB;Trusted_Connection=True;MultipleActiveResultSets=true";
        public SqlConnection MainConnection;
        public DBContext()
        {
            MainConnection = new SqlConnection(ConnectionInformation);
            MainConnection.Open();
        }
    }
}