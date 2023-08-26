using Patterson.model;
using Patterson.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Patterson.utils.MathUtils;

namespace Patterson.service.implementation
{
    internal class PattersonFunctionService : IPattersonFunctionService
    {


        public void Execute(Sample sample)
        {
            CalculateParams(sample.preExposurePeaksData);
           // CalculateParams(sample.postExposurePeaksData);
            sample.preExposurePs = CalculatePs(sample.preExposurePeaksData);
          //  sample.postExposurePs = CalculatePs(sample.postExposurePeaksData);
            
        }

        private List<double> CalculatePs(List<PeakData> peaksData)
        {
            List<double> ps = new List<double>();
            for (double i = 1.0; i <= 10.0 + double.Epsilon; i += 0.1)
            {
                double sum = 0;
                foreach (var peak in peaksData)
                {
                    double term = Math.Sin(2 * Math.PI * peak.inverseD * i) * peak.fSquared / (2 * Math.PI * peak.inverseD * i);
                    sum += term;
                }
                ps.Add(sum);
            }
            return ps;
        }

        private void CalculateParams(List<PeakData> peaksData)
        {
            CalculateSinThetta(peaksData);
            CalculateDn(peaksData);
            CalculatePlg(peaksData);
            CalculateFsquared(peaksData);
            CalculateInverseDs(peaksData);
        }

        private void CalculateDn(List<PeakData> peaksData)
        {
            //TEST STUB
            peaksData[0].dn = 2.545997017;
            peaksData[1].dn = 2.338227298;
            peaksData[2].dn = 2.281306682;
            peaksData[3].dn = 1.727690374;
            peaksData[4].dn = 1.378579131;
            peaksData[5].dn = 1.331590563;
        }

        private void CalculateInverseDs(List<PeakData> peaksData)
        {
            foreach (var peak in peaksData)
            {
                peak.inverseD = 1 / peak.dn;
            }
        }

        private void CalculateFsquared(List<PeakData> peaksData)
        {
            foreach (var peak in peaksData)
            {
                peak.fSquared = peak.intensity / peak.plg;
            }
        }

        private void CalculatePlg(List<PeakData> peaksData)
        {
            foreach (var peak in peaksData)
            {
                peak.plg = (1 + Squared(Math.Cos(ToRads(peak.theta)))) / (Math.Sin(ToRads(peak.theta)));
            }
        }

        private void CalculateSinThetta(List<PeakData> peaksData)
        {
            foreach (var peak in peaksData)
            {
                peak.sinThetta = Math.Sin(ToRads(peak.theta / 2));
            }
        }

    }
}
