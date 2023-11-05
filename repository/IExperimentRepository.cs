using Patterson.model;

namespace Patterson.repository
{
    public interface IExperimentRepository
    {
        Experiment CreateNewExperiment(Element element, string description);
    }
}
