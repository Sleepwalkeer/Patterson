using Patterson.model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Patterson
{
    public partial class ChartForm : Form
    {
        private Form1 startForm;
        private bool isPostPicUploaded;

        public ChartForm()
        {
            InitializeComponent();
            PostInit();
        }

        public void PlotChart(Sample sample)
        {
            Series series = new Series(sample.experiment.Element.Name + " pre exposed");
            if (isPostPicUploaded)
            {
                series = new Series(sample.experiment.Element.Name + " post exposed")
                {
                    Color = Color.Red
                };
            }
            series.ChartType = SeriesChartType.Line;

            for (int i = 0; i < sample.pattersonPeaks.Count; i++)
            {
                series.Points.AddXY(i, sample.pattersonPeaks[i].Pu);
            }
            chart1.Series.Add(series);
        }

        private void PostInit()
        {
            SetChartScalable();

            if (isPostPicUploaded)
            {
                button3.Visible = false;
            }
        }


        private void SetChartScalable()
        {
            chart1.Series.Clear();
            ChartArea CA = chart1.ChartAreas[0];
            CA.AxisX.ScaleView.Zoomable = true;
            CA.CursorX.AutoScroll = true;
            CA.CursorX.IsUserSelectionEnabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            isPostPicUploaded = false;
            startForm.UploadNewSamplePicButtonHandler();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            isPostPicUploaded = true;
            startForm.UploadPostExposurePicButtonHandler();
            this.Hide();
        }

        public void Run(Sample sample, Form1 callbackForm)
        {
            this.startForm = callbackForm;
            PlotChart(sample);
            this.Show();

            if (!isPostPicUploaded)
            {
                button3.Visible = true;
            }
            else
            {
                button3.Visible = false;
            }

        }
    }
}
