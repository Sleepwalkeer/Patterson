using Patterson.model;
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
        private ChartForm chartForm = new ChartForm();
        private readonly IImageProcessorService imageProcessorService = new ImageProcessorService();
        private readonly IPattersonFunctionService pattersonFunctionService = new PattersonFunctionService();
        private bool isPostExposure = false;
        private Experiment currentExperiment;
        public Form1()
        {
            InitializeComponent();

        }

        private void uploadPictureButton_Click(object sender, EventArgs e)
        {
            try
            {
                Image image = UploadPicture();
                if (image != null)
                {
                    pictureBox.Image = imageProcessorService.RenderImage(image);
                    execute.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while uploading the picture. Please, try again or choose another picture." + ex.Message);
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

            if (thetas.HasValue && lambda != -1 && comboBox1.SelectedIndex >= 0 && !string.IsNullOrEmpty(textBox5.Text))
            {
                double minTheta = thetas.Value.minTheta;
                double maxTheta = thetas.Value.maxTheta;
                peaks = imageProcessorService.ProcessImage(minTheta, maxTheta);

                if (currentExperiment == null)
                {
                    Element element = pattersonFunctionService.FindElementByName(comboBox1.SelectedItem.ToString());
                    Experiment experiment = pattersonFunctionService.CreateNewExperiment(element);
                    experiment.Description = textBox5.Text;
                    currentExperiment = experiment;
                }
                else
                {
                    foreach (var peak in peaks)
                    {
                        peak.IsUvExposed = true;
                    }
                }

                Sample sample = pattersonFunctionService.Execute(peaks, lambda, currentExperiment, isPostExposure);
                pattersonFunctionService.SaveData(sample);
                chartForm.Run(sample, this);
            }
            else if (comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("You haven't selected the sample element");
            }

            else if (string.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show("You haven't entered the experiments' description.");
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
                return (2 * lambda1 + lambda2) / 3;
            }
            return lambda1;
        }

        private bool Lambdasvalid()
        {
            bool isLambda1Valid = double.TryParse(textBox3.Text, out double lambda1);
            if (lambda1 <= 0)
            {
                MessageBox.Show("Lambda must be greater than zero.");
                return false;
            }
            if (checkBox1.Checked)
            {
                bool isLambda2Valid = double.TryParse(textBox4.Text, out double lambda2);
                if (lambda2 <= 0)
                {
                    MessageBox.Show("Lambda must be greater than zero.");
                    return false;
                }
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
            string[] metalElements = pattersonFunctionService.GetAllElementNames();
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
            textBox5.Visible = true;
            textBox5.Text = "";
            comboBox1.Visible = true;
            label3.Visible = true;
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
            isPostExposure = true;
            this.Show();
            pictureBox.Image = null;
            textBox5.Visible = false;
            comboBox1.Visible = false;
            label3.Visible = false;
            execute.Visible = false;
        }

        public void UploadNewSamplePicButtonHandler()
        {
            currentExperiment = null;
            isPostExposure = false;
            this.Show();
            ResetForm();
            UnlockParametersChange();
        }
    }
}
