using Patterson.model;
using Patterson.utils;
using System.Data.Entity;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using Npgsql;

namespace Patterson.persistence
{
    public class PattersonDBContext : DbContext
    {
        public PattersonDBContext() : base(GetConnectionString()) {}

        public DbSet<Element> Elements { get; set; }

        public DbSet<Experiment> Experiments { get; set; }

        public DbSet<PattersonPeak> PattersonPeaks { get; set; }

        public DbSet<PeakData> PeaksData { get; set; }


        private static string GetConnectionString()
        {
            string selectedConnectionStringName = ConfigurationManager.AppSettings["SelectedConnectionStringName"];
            return ConfigurationManager.ConnectionStrings[selectedConnectionStringName].ToString();
        }
    }
}
