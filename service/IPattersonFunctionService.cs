using Patterson.model;
using System.Collections.Generic;

namespace Patterson.service
{
    public interface IPattersonFunctionService
    {
        Sample Execute(List<PeakData> peaks, double lambda, Experiment experiment, bool isPostExposed);

        void SaveData(Sample sample);

        string[] GetAllElementNames();

        Experiment CreateNewExperiment(Element element, string description);

        Element FindElementByName(string name);
    }
}
