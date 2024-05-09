using Patterson.model;
using System.Data.Entity;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace Patterson.persistence
{
    public class PattersonDBContext : DbContext
    {
        public PattersonDBContext() : base(GetConnectionString()) { }

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
