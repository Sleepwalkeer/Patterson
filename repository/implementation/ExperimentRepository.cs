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

        public Experiment CreateNewExperiment(Element element)
        {
            try
            {
                Experiment experiment = new Experiment(Guid.NewGuid(), DateTime.Now, element);

                var experimentDbSet = context.Experiments;
                experimentDbSet.Add(experiment);
                context.SaveChanges();
                return experiment;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while creating a new experiment" + ex.Message);
                throw new ExperimentCreatingException(ex.Message);
            }
        }
    }
}
