using Patterson.model;
using Patterson.service;
using Patterson.service.implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Patterson.viewmodel
{
    public class MainFormViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ChartForm chartForm = ChartForm.GetInstance();
        private readonly IImageProcessorService imageProcessorService;
        private readonly IPattersonFunctionService pattersonFunctionService;
        private bool isPostExposure;
        private Experiment currentExperiment;

        public event EventHandler ViewDataRequested;

        private string minTheta;
        private string maxTheta;
        private string lambda1;
        private string lambda2;
        private string validationErrorMessage;
        private string experimentDescription;
        private string selectedElement;
        private string[] comboBoxItems;
        private bool executeButtonVisible;

        public MainFormViewModel()
        {
            imageProcessorService = new ImageProcessorService();
            pattersonFunctionService = new PattersonFunctionService();
            ComboBoxItems = pattersonFunctionService.GetAllElementNames();
            isPostExposure = false;
        }

        public void OnViewDataRequested()
        {
            ViewDataRequested?.Invoke(this, EventArgs.Empty);
        }


        public Experiment CurrentExperiment
        {
            get { return currentExperiment; }
            set
            {
                if (currentExperiment != value)
                {
                    currentExperiment = value;
                    OnPropertyChanged(nameof(CurrentExperiment));
                }
            }
        }

        public bool ExecuteButtonVisible
        {
            get { return executeButtonVisible; }
            set
            {
                if (executeButtonVisible != value)
                {
                    executeButtonVisible = value;
                    OnPropertyChanged(nameof(ExecuteButtonVisible));
                }
            }
        }
        public string[] ComboBoxItems
        {
            get { return comboBoxItems; }
            set
            {
                if (comboBoxItems != value)
                {
                    comboBoxItems = value;
                    OnPropertyChanged(nameof(ComboBoxItems));
                }
            }
        }
        public string MinTheta
        {
            get { return minTheta; }
            set
            {
                if (minTheta != value)
                {
                    minTheta = value;
                    OnPropertyChanged(nameof(MinTheta));
                }
            }
        }
        public string MaxTheta
        {
            get { return maxTheta; }
            set
            {
                if (maxTheta != value)
                {
                    maxTheta = value;
                    OnPropertyChanged(nameof(MaxTheta));
                }
            }
        }

        public string Lambda1
        {
            get { return lambda1; }
            set
            {
                if (lambda1 != value)
                {
                    lambda1 = value;
                    OnPropertyChanged(nameof(Lambda1));
                }
            }
        }
        public string Lambda2
        {
            get { return lambda2; }
            set
            {
                if (lambda2 != value)
                {
                    lambda2 = value;
                    OnPropertyChanged(nameof(Lambda2));
                }
            }
        }
        public string ValidationErrorMessage
        {
            get { return validationErrorMessage; }
            set
            {
                if (validationErrorMessage != value)
                {
                    validationErrorMessage = value;
                    OnPropertyChanged(nameof(ValidationErrorMessage));
                }
            }
        }
        public string SelectedElement
        {
            get { return selectedElement; }
            set
            {
                if (selectedElement != value)
                {
                    selectedElement = value;
                    OnPropertyChanged(nameof(SelectedElement));
                }
            }
        }
        public string ExperimentDescription
        {
            get { return experimentDescription; }
            set
            {
                if (experimentDescription != value)
                {
                    experimentDescription = value;
                    OnPropertyChanged(nameof(ExperimentDescription));
                }
            }
        }

        public void UploadPicture(PictureBox pictureBox)
        {
            try
            {
                string imagePath = OpenFile();
                if (!string.IsNullOrEmpty(imagePath))
                {
                    Image image = Image.FromFile(imagePath);
                    pictureBox.Image = imageProcessorService.RenderImage(image);
                    ExecuteButtonVisible = true;
                    MessageBox.Show("Please select the area of the graph for which you would like to perform calculations, taking into account the theta angles.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while uploading the picture. Please try again or choose another picture." + ex.Message);
            }
        }

        public Sample ExecutePattersonMethod(Point start, Point end)
        {
            var thetas = GetThetas();
            var lambda = GetLambda();

            if (start.Equals(end))
            {
                ValidationErrorMessage = "You haven't selected the chart area to compute";
                return null;
            }

            List<PeakData> peaks;

            if (thetas.HasValue && lambda != -1 && !string.IsNullOrEmpty(SelectedElement) && !string.IsNullOrEmpty(ExperimentDescription))
            {
                double minTheta = thetas.Value.minTheta;
                double maxTheta = thetas.Value.maxTheta;
                peaks = imageProcessorService.ProcessImage(minTheta, maxTheta, start, end);


                if (currentExperiment == null)
                {
                    Element element = pattersonFunctionService.FindElementByName(SelectedElement);
                    Experiment experiment = pattersonFunctionService.CreateNewExperiment(element, ExperimentDescription);
                    currentExperiment = experiment;
                    //peaks = useTestPeaksData();
                    isPostExposure = false;
                }
                else
                {
                    foreach (var peak in peaks)
                    {
                        peak.IsUvExposed = true;
                    }
                    isPostExposure = true;
                }

                Sample sample = pattersonFunctionService.Execute(peaks, lambda, currentExperiment, isPostExposure);
                pattersonFunctionService.SaveData(sample);
                executeButtonVisible = false;
                return sample;
            }
            else if (string.IsNullOrEmpty(SelectedElement))
            {
                ValidationErrorMessage = "You haven't selected the sample element";
            }

            else if (string.IsNullOrEmpty(ExperimentDescription))
            {
                ValidationErrorMessage = "You haven't entered the experiments' description.";
            }
            return null;
        }

        public void PopulateComboBoxItems()
        {
            ComboBoxItems = pattersonFunctionService.GetAllElementNames();
        }

        private string OpenFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.emf;*.png;*.bmp;*.jpg|All Files|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileName;
                }
                return null;
            }
        }

        private (double minTheta, double maxTheta)? GetThetas()
        {
            if (ThetasValid())
            {
                if (double.TryParse(MinTheta, out double minTheta) && double.TryParse(MaxTheta, out double maxTheta))
                {
                    return (minTheta, maxTheta);
                }
            }
            return null;
        }

        private bool ThetasValid()
        {
            const double minValue = 0;
            const double maxValue = 360;

            if (!double.TryParse(MinTheta, out double minTheta) || !double.TryParse(MaxTheta, out double maxTheta))
            {
                ValidationErrorMessage = "Invalid input, theta is invalid";
                return false;
            }

            if (minTheta < minValue || maxTheta > maxValue)
            {
                ValidationErrorMessage = "Minimum or maximum theta is out of range";
                return false;
            }

            if (minTheta >= maxTheta)
            {
                ValidationErrorMessage = "Minimum cannot be greater or equal to maximum";
                return false;
            }

            ValidationErrorMessage = null;
            return true;
        }

        private double GetLambda()
        {
            if (!Lambdasvalid())
            {
                ValidationErrorMessage = "Incorrect input. Lambda is invalid";
                return -1;
            }
            double lambda1 = double.Parse(Lambda1);

            if (!string.IsNullOrEmpty(Lambda2))
            {
                double lambda2 = double.Parse(Lambda2);
                return (2 * lambda1 + lambda2) / 3;
            }
            return lambda1;
        }

        private bool Lambdasvalid()
        {
            bool isLambda1Valid = double.TryParse(Lambda1, out double lambda1Value);

            if (lambda1Value <= 0)
            {
                ValidationErrorMessage = "Lambda must be greater than zero.";
                return false;
            }

            if (!string.IsNullOrEmpty(Lambda2))
            {
                bool isLambda2Valid = double.TryParse(Lambda2, out double lambda2Value);

                if (lambda2Value <= 0)
                {
                    ValidationErrorMessage = "Lambda must be greater than zero.";
                    return false;
                }

                return isLambda1Valid && isLambda2Valid;
            }

            return isLambda1Valid;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void HandleNewExperimentRequested(object sender, EventArgs e)
        {
            CurrentExperiment = null;
        }


        public Image test(Point start, Point end)
        {
            var thetas = GetThetas();
            double minTheta = thetas.Value.minTheta;
            double maxTheta = thetas.Value.maxTheta;
            return imageProcessorService.testProcessImage(minTheta, maxTheta, start, end);
        }

        public List<PeakData> useTestPeaksData()
        {
            var peaks = new List<PeakData>();
            PeakData peak1 = new PeakData(14.6766 * 3.3, 35.25);
            PeakData peak2 = new PeakData(164.0836 * 3.3, 38.5);
            PeakData peak3 = new PeakData(20.4399 * 3.3, 39.5);
            PeakData peak4 = new PeakData(40.0695 * 3.3, 53);
            PeakData peak5 = new PeakData(10.5444 * 3.3, 68);
            PeakData peak6 = new PeakData(58.2954 * 3.3, 70.75);
            peaks.Add(peak1);
            peaks.Add(peak2);
            peaks.Add(peak3);
            peaks.Add(peak4);
            peaks.Add(peak5);
            peaks.Add(peak6);
            return peaks;
        }
    }
}
