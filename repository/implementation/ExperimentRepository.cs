using Patterson.exception;
using Patterson.model;
using Patterson.persistence;
using System;
using System.Windows.Forms;

namespace Patterson.repository.implementation
{
    public class ExperimentRepository : IExperimentRepository
    {
        private readonly PattersonDBContext context;

        public ExperimentRepository(PattersonDBContext context)
        {
            this.context = context;
        }

        public Experiment CreateNewExperiment(Element element, string description)
        {
                Experiment experiment = new Experiment(Guid.NewGuid(), DateTime.Now, element, description);
                context.Experiments.Add(experiment);
                context.SaveChanges();
                return experiment;
        }
    }
}
