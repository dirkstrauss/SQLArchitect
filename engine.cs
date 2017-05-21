using SQLArchitect.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLArchitect
{
    public static class engine
    {
        public static string DatabaseName { get; set; }
        public static string DatabaseConnectionString { get; set; }

        public static string FileName { get; set; }

        private static DataSet dsTables;
        private static DataSet dsProcedures;
        private static DataSet dsScalarFunctions;
        private static string _newFilePath;
        public static async Task ProcessFiles(string path)
        {
            _newFilePath = path;
            foreach (string file in Directory.GetFiles(path, "*.sql"))
            {
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(fs, Encoding.UTF8))
                    {
                        string text = await reader.ReadToEndAsync();
                        MatchObjects(text);
                    }
                }
            }
        }

        private static void MatchObjects(string fileText)
        {
            string sOutputText = "";
            string sObject = "";

            if (fileText.ToUpper().Contains("ALTER TABLE"))
            {
                if (dsTables == null)
                    // Read the whole SQL statement
                    dsTables = DataController.GetDataTables(DatabaseConnectionString);

                foreach(DataRow item in dsTables.Tables["DataTables"].Rows)
                {
                    string tableName = item["name"].ToString();
                    if (fileText.Contains(item["name"].ToString()))
                    {
                        sOutputText = fileText;
                        sObject = $"ALTER TABLE {tableName}";
                    }
                }
            }
            else if (fileText.ToUpper().Contains("CREATE PROCEDURE"))
            {
                if (dsProcedures == null)
                    // Get only the Stored Procedure name
                    dsProcedures = DataController.GetStoredProcedures(DatabaseConnectionString);

                foreach (DataRow item in dsProcedures.Tables["Procedures"].Rows)
                {
                    string sprocName = item["name"].ToString();
                    if (fileText.Contains(sprocName))
                    {
                        StringBuilder sb = new StringBuilder();
                        
                        sb.Append(" IF EXISTS ( SELECT * ");
                        sb.Append("             FROM   sysobjects "); 
                        sb.Append($"             WHERE  id = object_id(N'[dbo].[{sprocName}]') "); 
                        sb.Append("                    and OBJECTPROPERTY(id, N'IsProcedure') = 1 ) ");
                        sb.Append(" BEGIN ");
                        sb.Append($"     DROP PROCEDURE [dbo].[{sprocName}] ");
                        sb.Append(" END ");

                        sOutputText = sb.ToString();
                        sObject = $"CREATE PROCEDURE {sprocName}";
                    }
                }
            }
            else if (fileText.ToUpper().Contains("CREATE FUNCTION"))
            {
                if (dsScalarFunctions == null)
                    // Get only the Function name
                    dsScalarFunctions = DataController.GetScalarFunctions(DatabaseConnectionString);

                foreach (DataRow item in dsScalarFunctions.Tables["ScalarFunctions"].Rows)
                {
                    string funcName = item["name"].ToString();
                    if (fileText.Contains(item["name"].ToString()))
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.Append(" IF EXISTS ( SELECT * "); 
                        sb.Append("             FROM   sysobjects "); 
                        sb.Append($"             WHERE  id = object_id(N'[dbo].[{funcName}]') "); 
                        sb.Append("                    and OBJECTPROPERTY(id, N'IsScalarFunction') = 1 ) ");
                        sb.Append(" BEGIN ");
                        sb.Append($"     DROP FUNCTION [dbo].[{funcName}] ");
                        sb.Append(" END ");

                        sOutputText = sb.ToString();
                        sObject = $"CREATE FUNCTION {funcName}";
                    }
                }
            }

            AppendToFile(sOutputText, sObject);
        }

        private static void AppendToFile(string textToAppend, string objectName)
        {
            string path = Path.Combine(_newFilePath, FileName);

            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine($"-- Script generated by SQL Architect - {DateTime.Now.ToShortDateString() + Environment.NewLine + Environment.NewLine}");
                }
            }
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine($"-- {objectName}");
                sw.WriteLine(textToAppend);
                sw.WriteLine("-- end" + Environment.NewLine + Environment.NewLine);
            }
        }
    }
}
