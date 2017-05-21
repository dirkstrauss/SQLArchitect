using SQLArchitect.Data;
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
        public static DataSet GetStoredProcedures(string connectionString)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $"SELECT * FROM sys.objects WHERE type_desc = 'SQL_STORED_PROCEDURE'";

            return DatabaseHelper.GetDataSet(cmd, "Procedures", connectionString);
        }        
    }
}
