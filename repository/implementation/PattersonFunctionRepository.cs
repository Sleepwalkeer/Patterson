using Npgsql;
using Patterson.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterson.repository.implementation
{
    internal class PattersonFunctionRepository : IPattersonFunctionRepository
    {
        private string connectionString;

        public PattersonFunctionRepository()
        {
            connectionString = SetConnectionString();
        }

        public void CreatePeaksTableIfNotExists()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = connection;

                        // Check if the "peaks" table already exists
                        cmd.CommandText = "SELECT to_regclass('public.peaks')::text";

                        object result = cmd.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                        {
                            // The "peaks" table does not exist, so create it
                            cmd.CommandText = "CREATE TABLE public.peaks " +
                                              "(id serial PRIMARY KEY, " +
                                              "intensity double precision, " +
                                              "theta double precision, " +
                                              "element varchar(50))";

                            cmd.ExecuteNonQuery();
                            Console.WriteLine("The 'peaks' table has been created.");
                        }
                        else
                        {
                            Console.WriteLine("The 'peaks' table already exists.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating 'peaks' table: {ex.Message}");
            }
        }

        public void savePeaks()
        {
            CreatePeaksTableIfNotExists();
        }

        private string SetConnectionString()
        {
            string host = PropertyReader.GetProperty("Host");
            int port = int.Parse(PropertyReader.GetProperty("Port"));
            string database = PropertyReader.GetProperty("Database");
            string username = PropertyReader.GetProperty("Username");
            string password = PropertyReader.GetProperty("Password");

            return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
        }
    }
}
