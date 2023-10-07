using Patterson.model;
using Patterson.utils;
using System.Data.Entity;

namespace Patterson.persistence
{
    public class PattersonDBContext : DbContext
    {
        public PattersonDBContext() : base(SetConnectionString()) { }

        public DbSet<Element> Elements { get; set; }
        public DbSet<Experiment> Experiments { get; set; }
        public DbSet<PattersonPeak> PattersonPeaks { get; set; }
        public DbSet<PeakData> PeakDataList { get; set; }

        private static string SetConnectionString()
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
