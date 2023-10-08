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
                experiment = experiment,
                pattersonPeaks = new List<PattersonPeak>()
            };
            CalculateParams(sample, lambda);
            FillOutAdditionalInfo(sample, experiment, isPostExposed);
            return sample;

        }

        private void CalculatePattersonPeaks(Sample sample)
        {
            List<double> ps = new List<double>();
            for (double i = 1.0; i <= 10.0 + double.Epsilon; i += sample.experiment.Element.deltaR)
            {
                double sum = 0;
                foreach (var peak in sample.peaksData)
                {
                    double term = Math.Sin(2 * Math.PI * peak.OneOverD * i) * peak.FSquared / (2 * Math.PI * peak.OneOverD * i);
                    sum += term;
                }
                sample.pattersonPeaks.Add(new PattersonPeak(i, sum));
            }
        }

        private void CalculateParams(Sample sample, double lambda)
        {
            CalculateSinThetta(sample.peaksData);
            CalculateDn(sample.peaksData, lambda);
            CalculatePlg(sample.peaksData);
            CalculateFsquared(sample.peaksData);
            CalculateInverseDs(sample.peaksData);
            CalculatePattersonPeaks(sample);
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

            foreach (var pattersonPeak in sample.pattersonPeaks)
            {
                pattersonPeak.Experiment = experiment;
                pattersonPeak.ExperimentId = experiment.Id;
                pattersonPeak.IsUvExposed = isPostExposed;
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
