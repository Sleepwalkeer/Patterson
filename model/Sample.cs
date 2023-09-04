using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterson.model
{
    public class Sample
    {
        public Sample(List<PeakData> peaksData)
        {
            this.peaksData = peaksData;
        }

        public String element { get; set; }
        public List<PeakData> peaksData { get; set; }
        public List<double> ps { get; set; }
    }
}
