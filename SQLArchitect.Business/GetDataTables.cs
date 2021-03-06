﻿using SQLArchitect.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLArchitect.Business
{
    public static partial class DataController
    {
        public static DataSet GetDataTables(string connectionString)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM sys.objects WHERE type_desc = 'USER_TABLE'"; // $"SELECT TABLE_NAME FROM [{databaseName}].INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
            
            return DatabaseHelper.GetDataSet(cmd, "DataTables", connectionString);
        }        
    }
}
