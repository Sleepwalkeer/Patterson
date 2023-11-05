using Npgsql;
using Patterson.model;
using Patterson.persistence;
using Patterson.utils;
using System;
using System.Configuration;
using System.Data.Entity.Migrations;

namespace Patterson.repository.implementation
{
    internal class MainRepository : IMainRepository
    {
        private readonly IElementRepository elementRepository;
        private readonly IExperimentRepository experimentRepository;
        private readonly IPattersonPeakRepository pattersonPeakRepository;
        private readonly IPeakDataRepository peakDataRepository;
        private readonly PattersonDBContext context;

        public MainRepository()
        {
            UpdateMigrations();
            context = new PattersonDBContext();
            elementRepository = new ElementRepository(context);
            experimentRepository = new ExperimentRepository(context);
            pattersonPeakRepository = new PattersonPeakRepository(context);
            peakDataRepository = new PeakDataRepository(context);

        }

        private void UpdateMigrations()
        {
            var configuration = new Patterson.Migrations.Configuration();
            var migrator = new DbMigrator(configuration);
            migrator.Update();

        }

        public Experiment CreateNewExperiment(Element element, string description)
        {
            return experimentRepository.CreateNewExperiment(element, description);
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
    }
}
