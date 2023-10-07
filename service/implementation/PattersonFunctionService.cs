using Patterson.model;
using Patterson.repository;
using Patterson.repository.implementation;
using System;
using System.Collections.Generic;
using static Patterson.utils.MathUtils;

namespace Patterson.service.implementation
{
    internal class PattersonFunctionService : IPattersonFunctionService
    {
        private readonly IMainRepository repository = new MainRepository();


        public Sample Execute(List<PeakData> peaks, double lambda, Experiment experiment, bool isPostExposed)
        {
            Sample sample = new Sample()
            {
                peaksData = peaks,
                experiment = experiment
            };
            CalculateParams(sample.peaksData, lambda);
            sample.ps = CalculatePs(sample.peaksData, experiment.Element.deltaR);
            FillOutAdditionalInfo(sample, experiment, isPostExposed);
            return sample;

        }

        private List<double> CalculatePs(List<PeakData> peaksData, double deltaR)
        {
            List<double> ps = new List<double>();
            for (double i = 1.0; i <= 10.0 + double.Epsilon; i += deltaR)
            {
                double sum = 0;
                foreach (var peak in peaksData)
                {
                    double term = Math.Sin(2 * Math.PI * peak.OneOverD * i) * peak.FSquared / (2 * Math.PI * peak.OneOverD * i);
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
                peak.DOverN = lambda / 2 / peak.sinThetta;
            }
        }

        private void CalculateInverseDs(List<PeakData> peaksData)
        {
            foreach (var peak in peaksData)
            {
                peak.OneOverD = 1 / peak.DOverN;
            }
        }

        private void CalculateFsquared(List<PeakData> peaksData)
        {
            foreach (var peak in peaksData)
            {
                peak.FSquared = peak.Intensity / peak.Plg;
            }
        }

        private void CalculatePlg(List<PeakData> peaksData)
        {
            foreach (var peak in peaksData)
            {
                peak.Plg = (1 + Squared(Math.Cos(ToRads(peak.DoubleTheta)))) / (Math.Sin(ToRads(peak.DoubleTheta)));
            }
        }

        private void CalculateSinThetta(List<PeakData> peaksData)
        {
            foreach (var peak in peaksData)
            {
                peak.sinThetta = Math.Sin(ToRads(peak.DoubleTheta / 2));
            }
        }

        private void FillOutAdditionalInfo(Sample sample, Experiment experiment, bool isPostExposed)
        {
            for (int i = 0; i < sample.peaksData.Count; i++)
            {
                PeakData peakData = sample.peaksData[i];
                peakData.PeakId = i;
                peakData.Experiment = experiment;
                peakData.ExperimentId = experiment.Id;
                peakData.IsUvExposed = isPostExposed;
            }
        }

        public void InitializeDB()
        {
            repository.InitializeDB();
        }

        public Element FindElementByName(string name)
        {
            return repository.FindElementByName(name);
        }

        public Experiment CreateNewExperiment(Element element)
        {
            return repository.CreateNewExperiment(element);
        }

        public string[] GetAllElementNames()
        {
            return repository.GetAllElementNames();
        }

        public void SaveData(Sample sample)
        {
            repository.SaveData(sample);
        }
    }
}
