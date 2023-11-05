using Patterson.model;

namespace Patterson.repository
{
    public interface IMainRepository
    {
        void SaveData(Sample sample);
        Element FindElementByName(string name);

        Experiment CreateNewExperiment(Element element, string description);

        string[] GetAllElementNames();
    }
}
