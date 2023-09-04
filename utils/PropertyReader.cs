using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterson.utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class PropertyReader
    {
        private static Dictionary<string, string> properties = new Dictionary<string, string>();
        private const string FILE_PATH = "applicationProperties.txt";

        static PropertyReader()
        {
            LoadProperties(FILE_PATH);
        }

        public static void LoadProperties(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();
                        properties[key] = value;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading properties file: " + ex.Message);
            }
        }

        public static string GetProperty(string key)
        {
            if (properties.ContainsKey(key))
            {
                return properties[key];
            }
            else
            {
                return null;
            }
        }
    }
}
