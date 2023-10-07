using System.Collections.Generic;

namespace Patterson.model
{
    public class Sample
    {
        public Experiment experiment { get; set; }
        public List<PeakData> peaksData { get; set; }

        public List<PattersonPeak> pattersonPeaks { get; set; }
        public List<double> ps { get; set; }
    }
}
