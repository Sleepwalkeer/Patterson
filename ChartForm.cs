using Patterson.model;
using Patterson.viewmodel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Patterson
{
    public partial class ChartForm : Form
    {

        private static ChartForm instance;
        private readonly ChartFormViewModel viewModel;
        private Form1 startForm;
        private DataBaseForm dataBaseForm = DataBaseForm.GetInstance();
        private bool isPostPicUploaded;

        public ChartFormViewModel ViewModel => viewModel;

        public ChartForm()
        {
            viewModel = ChartFormViewModel.Instance;
            InitializeComponent();
            PostInit();

            newExperimentButton.Click += (sender, e) => viewModel.OnCreateNewExperimentRequested();
            viewModel.ViewDataRequested += ViewModel_ViewDataRequested;
        }

        public static ChartForm GetInstance()
        {
            if (instance == null)
            {
                instance = new ChartForm();
            }
            return instance;
        }

        private void ViewModel_ViewDataRequested(object sender, EventArgs e)
        {
                this.Hide();
                dataBaseForm.Show();
        }

        public void PlotChart(Sample sample)
        {
            Series series = new Series(sample.experiment.Element.Name + " pre exposed");
            if (sample.pattersonPeaks[0].IsUvExposed)
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

        public void PlotChartFromDataBase(Sample sample)
        {
            List<PattersonPeak> uvExposedPeaks = sample.pattersonPeaks.Where(peak => peak.IsUvExposed).ToList();
            List<PattersonPeak> nonUvExposedPeaks = sample.pattersonPeaks.Where(peak => !peak.IsUvExposed).ToList();
            sample.pattersonPeaks = nonUvExposedPeaks;
            PlotChart(sample);
            sample.pattersonPeaks = uvExposedPeaks;
            PlotChart(sample);

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
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            isPostPicUploaded = true;
            startForm.UploadPostExposurePicButtonHandler();
            this.Hide(); //наверное надо вьюмодель звать
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

        private void button2_Click(object sender, EventArgs e)
        {
            ViewModel.OnViewDataRequested();
        }
    }
}
