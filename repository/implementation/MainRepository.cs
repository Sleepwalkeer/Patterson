using Npgsql;
using Patterson.model;
using Patterson.persistence;
using Patterson.utils;
using System;

namespace Patterson.repository.implementation
{
    internal class MainRepository : IMainRepository
    {
        private readonly IElementRepository elementRepository;
        private readonly IExperimentRepository experimentRepository;
        private readonly IPattersonPeakRepository pattersonPeakRepository;
        private readonly IPeakDataRepository peakDataRepository;
        private readonly PattersonDBContext context;

        private string connectionString;

        public MainRepository()
        {
            connectionString = SetConnectionString();
            context = new PattersonDBContext();
            elementRepository = new ElementRepository(context);
            experimentRepository = new ExperimentRepository(context);
            pattersonPeakRepository = new PattersonPeakRepository(context);
            peakDataRepository = new PeakDataRepository(context);
            InitializeDB();

        }

        public void InitializeDB()
        {
            bool existed = CreateElementTableIfNotExist();
            if (!existed)
            {
                elementRepository.Populate();
            }
            CreateExperimentTableIfNotExist();
            CreatePattersonPeakTableIfNotExist();
            CreatePeakDataTableIfNotExist();
        }

        public void CreatePeakDataTableIfNotExist()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT to_regclass('public.peak_data')::text";

                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                    {
                        cmd.CommandText = "CREATE TABLE peak_data (" +
                   "experiment_id uuid, " +
                   "is_uv_exposed BOOLEAN, " +
                   "peak_id INT, " +
                   "Intensity DOUBLE PRECISION, " +
                   "double_theta DOUBLE PRECISION, " +
                   "plg DOUBLE PRECISION, " +
                   "f_squared DOUBLE PRECISION, " +
                   "d_over_n DOUBLE PRECISION, " +
                   "one_over_d DOUBLE PRECISION, " +
                   "PRIMARY KEY (experiment_id, is_uv_exposed, peak_id), " +
                   "FOREIGN KEY (experiment_id) REFERENCES experiment(id));";

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        public bool CreateElementTableIfNotExist()
        {
            bool exists = true;
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT to_regclass('public.element')::text";

                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                    {
                        exists = false;
                        cmd.CommandText = "CREATE TABLE element (" +
                   "id UUID DEFAULT gen_random_uuid() PRIMARY KEY, " +
                   "name VARCHAR(255), " +
                   "deltaR DOUBLE PRECISION" +
                   ");";

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return exists;
        }

        public void CreatePattersonPeakTableIfNotExist()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT to_regclass('public.patterson_peak')::text";

                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                    {
                        cmd.CommandText = "CREATE TABLE patterson_peak (" +
                   "exp_id uuid, " +
                   "is_uv_exposed BOOLEAN, " +
                   "u DOUBLE PRECISION, " +
                   "pu DOUBLE PRECISION, " +
                   "PRIMARY KEY (exp_id, is_uv_exposed, u, pu), " +
                   "FOREIGN KEY (exp_id) REFERENCES experiment(id));";

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void CreateExperimentTableIfNotExist()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT to_regclass('public.experiment')::text";

                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                    {
                        cmd.CommandText = "CREATE TABLE experiment (" +
                    "id uuid PRIMARY KEY, " +
                    "datetime timestamp with time zone, " +
                    "elem_id uuid, " +
                    "description varchar(255), " +
                    "FOREIGN KEY (elem_id) REFERENCES element(id));";

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public Experiment CreateNewExperiment(Element element)
        {
            return experimentRepository.CreateNewExperiment(element);
        }

        public void SaveData(Sample sample)
        {
            peakDataRepository.SaveData(sample.peaksData);
            pattersonPeakRepository.SaveData(sample.pattersonPeaks);
        }

        public Element FindElementByName(String name)
        {
            return elementRepository.FindElementByName(name);
        }

        public string[] GetAllElementNames()
        {
            return elementRepository.GetAllElementNames();
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
