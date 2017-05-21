using SQLArchitect.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SQLArchitect.ExtensionMethods;

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

            string outputFile = Path.Combine(_newFilePath, FileName);
            if (File.Exists(outputFile))
            {
                if (File.Exists($"{outputFile}.sql"))
                {
                    File.Move(outputFile, $"{outputFile}_Copy_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.sql");
                }
                else
                    File.Move(outputFile, $"{outputFile}.sql");
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
                    if (fileText.ContainsInRelationTo(ScriptType.table, item["name"].ToString()))
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
                    if (fileText.ContainsInRelationTo(ScriptType.procedure, sprocName))
                    {
                        StringBuilder sb = new StringBuilder();
                        
                        sb.AppendLine(" IF EXISTS ( SELECT * ");
                        sb.AppendLine("             FROM sysobjects "); 
                        sb.AppendLine($"             WHERE id = object_id(N'[dbo].[{sprocName}]') "); 
                        sb.AppendLine("                    and OBJECTPROPERTY(id, N'IsProcedure') = 1 ) ");
                        sb.AppendLine(" BEGIN ");
                        sb.AppendLine($"     DROP PROCEDURE [dbo].[{sprocName}] ");
                        sb.AppendLine(" END ");

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
                    if (fileText.ContainsInRelationTo(ScriptType.function, funcName))
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.AppendLine(" IF EXISTS ( SELECT * "); 
                        sb.AppendLine("             FROM sysobjects "); 
                        sb.AppendLine($"             WHERE id = object_id(N'[dbo].[{funcName}]') "); 
                        sb.AppendLine("                    and OBJECTPROPERTY(id, N'IsScalarFunction') = 1 ) ");
                        sb.AppendLine(" BEGIN ");
                        sb.AppendLine($"     DROP FUNCTION [dbo].[{funcName}] ");
                        sb.AppendLine(" END ");

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
                sw.WriteLine(Environment.NewLine + Environment.NewLine); 
            }
        }
    }

    public static class ExtensionMethods
    {
        public enum ScriptType
        {
            procedure,
            function,
            table
        }
        public static bool ContainsInRelationTo(this string value, ScriptType type, string name)
        {
            bool isMatch = false;
            switch(type)
            {
                case ScriptType.procedure:
                    if (value.Contains($"CREATE PROCEDURE [dbo].[{name}]")) { return true; }
                    if (value.Contains($"CREATE PROCEDURE dbo.{name}")) { return true; }
                    if (value.Contains($"CREATE PROCEDURE {name}")) { return true; }
                    if (value.Contains($"CREATE PROCEDURE [{name}]")) { return true; }
                    break;

                case ScriptType.function:
                    if (value.Contains($"CREATE FUNCTION [dbo].[{name}]")) { return true; }
                    if (value.Contains($"CREATE FUNCTION dbo.{name}")) { return true; }
                    if (value.Contains($"CREATE FUNCTION {name}")) { return true; }
                    if (value.Contains($"CREATE FUNCTION [{name}]")) { return true; }
                    break;

                case ScriptType.table:
                    if (value.Contains($"ALTER TABLE [dbo].[{name}]")) { return true; }
                    if (value.Contains($"ALTER TABLE dbo.{name}")) { return true; }
                    if (value.Contains($"ALTER TABLE {name}")) { return true; }
                    if (value.Contains($"ALTER TABLE [{name}]")) { return true; }
                    break;
            }

            return isMatch;
        }
    }
}
