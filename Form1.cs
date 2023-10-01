using Patterson.model;
using Patterson.repository;
using Patterson.repository.implementation;
using Patterson.service;
using Patterson.service.implementation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Patterson
{
    public partial class Form1 : Form
    {
        private IPattersonFunctionRepository repository = new PattersonFunctionRepository();
        private ChartForm chartForm = new ChartForm();
        private readonly IImageProcessorService imageProcessorService = new ImageProcessorService();
        private readonly IPattersonFunctionService pattersonFunctionService = new PattersonFunctionService();
        public Form1()
        {
            InitializeComponent();
            repository = new PattersonFunctionRepository();
            repository.savePeaks();
        }

        private void uploadPictureButton_Click(object sender, EventArgs e)
        {
            try
            {
                Image image = UploadPicture();
                pictureBox.Image = imageProcessorService.RenderImage(image);
                execute.Visible = true;
        }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while uploading the picture. Please, try again or choose another picture."  + ex);
            }
}

        private Image UploadPicture()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.emf|All Files|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string imagePath = openFileDialog.FileName;

                    Image emfImage = Image.FromFile(imagePath);
                    return Image.FromFile(imagePath);
                }
                return null;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox4.Visible = true;
            }
            else
            {
                textBox4.Visible = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox4.Visible = false;
            execute.Visible = false;
            PopulateComboBox();

        }

        private void execute_Click(object sender, EventArgs e)
        {
            var thetas = GetThetas();
            var lambda = GetLambda();

            List<PeakData> peaks;

            if (thetas.HasValue && lambda > 0 && comboBox1.SelectedIndex >= 0)
            {
                double minTheta = thetas.Value.minTheta;
                double maxTheta = thetas.Value.maxTheta;
                peaks = imageProcessorService.ProcessImage(minTheta, maxTheta);
                //peaks = new List<PeakData>();
                //PeakData peak1 = new PeakData(14.6766, 35.25);
                //PeakData peak2 = new PeakData(164.0836, 38.5);
                //PeakData peak3 = new PeakData(20.4399, 39.5);
                //PeakData peak4 = new PeakData(40.0695, 53);
                //PeakData peak5 = new PeakData(10.5444, 68);
                //PeakData peak6 = new PeakData(58.2954, 70.75);
                //peaks.Add(peak1);
                //peaks.Add(peak2);
                //peaks.Add(peak3);
                //peaks.Add(peak4);
                //peaks.Add(peak5);
                //peaks.Add(peak6);
                Sample sample = pattersonFunctionService.Execute(peaks, lambda);
                sample.element = comboBox1.SelectedItem.ToString();
                chartForm.Run(sample, this);
                //DON't FORGET COMBOBOX
                //SEND TO DATABASE AND TO CHART FORM
            }
            else if (comboBox1.SelectedIndex == 0)
            {
                MessageBox.Show("You haven't selected the sample element");
            }
        }

            private double GetLambda()
        {
            if (!Lambdasvalid())
            {
                MessageBox.Show("Incorrect input. Lambda is invalid");
                return -1;
            }
            double lambda1 = double.Parse(textBox3.Text);
            if (checkBox1.Checked)
            {
                double lambda2 = double.Parse(textBox4.Text);
                return (2*lambda1+ lambda2)/3;
            }
            return lambda1;
        }

        private bool Lambdasvalid()
        {
            bool isLambda1Valid = double.TryParse(textBox3.Text, out double lambda1);
            if (checkBox1.Checked)
            {
                bool isLambda2Valid = double.TryParse(textBox4.Text, out double lambda2);
                return (isLambda1Valid && isLambda2Valid);
            }
            return isLambda1Valid;
        }

        private (double minTheta, double maxTheta)? GetThetas()
        {
            if (ThetasValid())
            {
                double minTheta = Convert.ToDouble(textBox1.Text);
                double maxTheta = Convert.ToDouble(textBox2.Text);
                return (minTheta, maxTheta);
            }
            else
            {
                return null;
            }
        }

        private bool ThetasValid()
        {
            const double minValue = 20;
            const double maxValue = 120;
            const double minDifference = 20;

            if (!double.TryParse(textBox1.Text, out double minTheta) || !double.TryParse(textBox2.Text, out double maxTheta))
            {
                MessageBox.Show("Invalid input, theta is invalid");
                return false;
            }

            if (minTheta < minValue || maxTheta > maxValue)
            {
                MessageBox.Show("Minimum or maximum theta is out of range");
                return false;
            }

            if (minTheta >= maxTheta)
            {
                MessageBox.Show("Minimum cannot be greater or equal to maximum");
                return false;
            }

            if (maxTheta - minTheta < minDifference)
            {
                MessageBox.Show("Thetas should be at least 20 points apart");
                return false;
            }

            return true;
        }

        private void PopulateComboBox()
        {
            string[] metalElements = { "Cu", "Fe", "Au", "Ag","Sn","Zn","Ni","Fe","Pb","Mn","Al" };
            comboBox1.Items.AddRange(metalElements);
        }

        public void ResetForm()
        {
            pictureBox.Image = null;
            comboBox1.SelectedIndex = -1;
            checkBox1.Checked = false;
            textBox4.Visible = false;
            execute.Visible = false;
            textBox1.Text = "0";
            textBox2.Text = "120";
            textBox3.Text = "";
            textBox4.Text = "";
        }


        public void DisableParametersChange()
        {
            SetParametersChangeState(false);
        }

        public void UnlockParametersChange()
        {
            SetParametersChangeState(true);
        }

        private void SetParametersChangeState(bool state)
        {
            textBox1.Enabled = state;
            textBox2.Enabled = state;
            textBox3.Enabled = state;
            textBox4.Enabled = state;
            checkBox1.Enabled = state;
            comboBox1.Enabled = state;
        }

        public void UploadPostExposurePicButtonHandler()
        {
            this.Show();
            pictureBox.Image = null;
            DisableParametersChange();
            execute.Visible = false;
        }

        public void UploadNewSamplePicButtonHandler()
        {
            this.Show();
            ResetForm();
            UnlockParametersChange();
        }
    }
}
