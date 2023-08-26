using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterson.model
{
    internal class Sample
    {
        public List<PeakData> preExposurePeaksData { get; set; }
        public List<PeakData> postExposurePeaksData { get; set; }
        public List<double> preExposurePs { get; set; }
        public List<double> postExposurePs { get; set; }
    }
}
