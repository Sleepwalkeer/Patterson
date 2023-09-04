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


        public Sample Execute(List<PeakData> peaks, double lambda)
        {
            Sample sample = new Sample(peaks);
            CalculateParams(sample.peaksData, lambda);
            sample.ps = CalculatePs(sample.peaksData);
            return sample;
            
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

        private void CalculateParams(List<PeakData> peaksData, double lambda)
        {
            CalculateSinThetta(peaksData);
            CalculateDn(peaksData, lambda);
            CalculatePlg(peaksData);
            CalculateFsquared(peaksData);
            CalculateInverseDs(peaksData);
        }

        private void CalculateDn(List<PeakData> peaksData, double lambda)
        {
            foreach (var peak in peaksData)
            {
                peak.dn = lambda / 2 / peak.sinThetta;
            }
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
