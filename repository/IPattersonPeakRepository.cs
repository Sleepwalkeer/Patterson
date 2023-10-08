using Patterson.model;
using System.Collections.Generic;

namespace Patterson.repository
{
    internal interface IPattersonPeakRepository
    {
        void SaveData(List<PattersonPeak> pattersonPeaks);
    }
}
