using Patterson.model;
using System.Collections.Generic;

namespace Patterson.service
{
    internal interface IPattersonFunctionService
    {
        Sample Execute(List<PeakData> peaks, double lambda, Experiment experiment, bool isPostExposed);

        void SaveData(Sample sample);

        string[] GetAllElementNames();

        Experiment CreateNewExperiment(Element element);

        Element FindElementByName(string name);

        void InitializeDB();
    }
}
