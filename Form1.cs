using Patterson.model;
using Patterson.service;
using Patterson.service.implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Patterson
{
    public partial class Form1 : Form
    {
        private readonly IImageProcessorService imageProcessorService = new ImageProcessorService();
        private readonly PattersonFunctionService pattersonFunctionService = new PattersonFunctionService();
        public Form1()
        {
            InitializeComponent();
        }

        private void uploadPictureButton_Click(object sender, EventArgs e)
        {
            try
            {
                Image image = UploadPicture();
                DisplayPicture(image);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while uploading the picture. Please, try again or choose another picture.");
            }
        }

        private void DisplayPicture(Image image)
        {
                pictureBox.Image = imageProcessorService.RenderImage(image);
        }

        private Image UploadPicture()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.emf;*.png;*.gif|All Files|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string imagePath = openFileDialog.FileName;

                    Image emfImage = Image.FromFile(imagePath);

                    return Image.FromFile(imagePath);
                }
                return null;
            }
        }
    }
}
