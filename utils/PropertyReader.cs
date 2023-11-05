namespace Patterson.utils
{
    using Patterson.exception;
    using Patterson.model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;

    public static class PropertyReader
    {
        private static Dictionary<string, string> properties = new Dictionary<string, string>();
        private const string ELEMENTS_FILE_PATH = "elements.txt";


        public static List<Element> getAllElements()
        {
            try
            {
                string[] lines = File.ReadAllLines(ELEMENTS_FILE_PATH);
                List<Element> elements = new List<Element>();

                foreach (string line in lines)
                {
                    string[] parts = line.Split(' ');
                    if (parts.Length == 2)
                    {
                        string name = parts[0].Trim();
                        double deltaR = Double.Parse(parts[1].Trim());
                        elements.Add(new Element(name, deltaR));
                    }
                }
                return elements;
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error getting list of elements from property file.");
                throw new PropertyParsingException(ex.Message);
            }
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
                Console.WriteLine("Error reading property file");
                throw new PropertyParsingException(ex.Message);
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
