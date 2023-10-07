using Patterson.model;

namespace Patterson.repository
{
    public interface IMainRepository
    {
        void SaveData(Sample sample);
        void InitializeDB();

        Element FindElementByName(string name);

        Experiment CreateNewExperiment(Element element);

        string[] GetAllElementNames();
    }
}
