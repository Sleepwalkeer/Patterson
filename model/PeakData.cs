using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterson.model
{
    public class PeakData
    {
        public double intensity { get; set; }
        public double theta { get; set; }
        public double sinThetta { get; set; }
        public double dn { get; set; }
        public double plg { get; set; }
        public double fSquared { get; set; }
        public double inverseD { get; set; }

        public PeakData(double intensity, double thetta)
        {
            this.intensity = intensity;
            this.theta = thetta;
        }

        public static List<PeakData> SortByTheta(List<PeakData> peakList)
        {
            // Use LINQ to sort the list by the Theta field
            List<PeakData> sortedList = peakList.OrderBy(peak => peak.theta).ToList();

            return sortedList;
        }
    }
}
