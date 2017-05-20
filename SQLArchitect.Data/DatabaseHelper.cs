using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLArchitect.Data
{
    public static class DatabaseHelper
    {
        public static string ConnectionString { get; private set; }
        public static DataSet GetDataSet(SqlCommand cmd, string tableName, string connectionString)
        {
            ConnectionString = connectionString;
            return DataAccess.GetDataSet(cmd, tableName);
        }
    }
    internal static class DataAccess
    {
        /// <summary>
        /// Calls the Stored Procedure for the SQL Command object passed as parameter
        /// </summary>
        /// <param name="cmd">SQL Command object containing Stored Procedure to call along with any parameters as needed</param>
        public static void ExecuteNonQuery(SqlCommand cmd)
        {
            Exception exception = null;
            
            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    cmd.Connection = connection;
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                if (exception != null) throw exception;
            }
        }

        /// <summary>
        /// Calls the Stored Procedure for the SQL Command object and returns a DataSet
        /// </summary>
        /// <param name="cmd">SQL Command object containing Stored Procedure to call along with any parameters as needed</param>
        /// <param name="tableName">The Table Name contained in the DataSet</param>
        /// <returns></returns>
        public static DataSet GetDataSet(SqlCommand cmd, string tableName)
        {
            Exception exception = null;
            
            DataSet dsData = new DataSet();
            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    cmd.Connection = connection;
                    cmd.Connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(da);
                    da.Fill(dsData, tableName);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                if (exception != null) throw exception;
            }
            return dsData;
        }

        /// <summary>
        /// Asynchronously calls the Stored Procedure for the SQL Command object and returns a DataSet
        /// </summary>
        /// <param name="cmd">SQL Command object containing Stored Procedure to call along with any parameters as needed</param>
        /// <param name="tableName">The Table Name contained in the DataSet</param>
        /// <returns></returns>
        public static async Task<DataTable> GetDataSetAsync(SqlCommand cmd, string tableName)
        {
            Exception exception = null;
            
            DataTable dataTable = new DataTable(tableName);
            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    cmd.Connection = connection;
                    await cmd.Connection.OpenAsync();

                    using (var dataReader = await cmd.ExecuteReaderAsync())
                    {
                        dataTable.Load(dataReader);
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                if (exception != null) throw exception;
            }
            return dataTable;
        }

        /// <summary>
        /// Calls a scalar to return an integer
        /// </summary>
        /// <param name="cmd">The SQL Command Object</param>
        /// <returns>Integer</returns>
        public static int ExecuteScalarInteger(SqlCommand cmd)
        {
            Exception exception = null;
            
            object retVal = null;
            int iReturn = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    cmd.Connection = connection;
                    cmd.Connection.Open();
                    retVal = cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                if (exception != null) throw exception;
                if (int.TryParse(retVal.ToString(), out iReturn))
                { }
            }
            return iReturn;
        }
    }
}
