using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLArchitect
{
    public static class engine
    {
        public static async Task ProcessFiles(string path)
        {
            foreach (string file in Directory.GetFiles(path, "*.sql"))
            {
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(fs, Encoding.UTF8))
                    {
                        string text = await reader.ReadToEndAsync();
                        
                    }
                }
            }
        }

        private static void MatchObjects(string fileText)
        {
            if (fileText.ToUpper().Contains("ALTER TABLE"))
            {
                // Read the whole SQL statement
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
