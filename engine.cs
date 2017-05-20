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

        private static DataSet dsTables;
        public static async Task ProcessFiles(string path)
        {
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

            if (fileText.ToUpper().Contains("ALTER TABLE"))
            {
                if (dsTables == null)
                    // Read the whole SQL statement
                    dsTables = DataController.GetDataTables(DatabaseName, DatabaseConnectionString);
            }
            else if (fileText.ToUpper().Contains("CREATE PROCEDURE"))
            {
                // Get only the Stored Procedure name
            }
            else if (fileText.ToUpper().Contains("CREATE FUNCTION"))
            {
                // Get only the Function name
            }

        }

    }
}
