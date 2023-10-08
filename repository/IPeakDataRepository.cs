using Patterson.model;
using System.Collections.Generic;

namespace Patterson.repository
{
    public interface IPeakDataRepository
    {
        void SaveData(List<PeakData> peaksData);
    }
}
