using Patterson.model;
using Patterson.persistence;
using Patterson.viewmodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Patterson
{
    public partial class DataBaseForm : Form
    {

        private readonly ChartFormViewModel viewModel;
        private static DataBaseForm instance;

        private PattersonDBContext dBContext = new PattersonDBContext();

        public ChartFormViewModel ViewModel => viewModel;

        public DataBaseForm()
        {
            InitializeComponent();
            viewModel = new ChartFormViewModel();

            newExperimentButton.Click += (sender, e) => viewModel.OnCreateNewExperimentRequested();
        }

        public static DataBaseForm GetInstance()
        {
            if(instance == null)
            {
                instance = new DataBaseForm();
            }
            return instance;
        }

        private void DataBaseForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            var data = dBContext.Experiments.Include(e => e.Element)
                .Select(e => new
                {
                    Id = e.Id,
                    DateTime = e.Date,
                    Description = e.Description,
                    ElementName = e.Element.Name
                })
                .ToList();
            dataGridView1.DataSource = data;
            dataGridView1.Columns["ID"].Visible = false;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                Guid selectedID = (Guid)dataGridView1.SelectedRows[0].Cells["ID"].Value;

                var peaksData = dBContext.PeaksData
                    .Select(p => new
                    {
                        Id = p.ExperimentId,
                        beforeExposure = !p.IsUvExposed,
                        peakId = p.PeakId,
                        intensity = p.Intensity,
                        DTheta = p.DoubleTheta,
                        PLG = p.Plg,
                        F = p.FSquared,
                        dOverN = p.DOverN,
                        OneOverD = p.OneOverD
                    })
                    .Where(p => p.Id == selectedID)
                    .ToList();

                dataGridView2.DataSource = peaksData;
                dataGridView2.Columns["Id"].Visible = false;


                var pattersonPeaksData = dBContext.PattersonPeaks
                    .Select(peak => new
                    {
                        Id = peak.ExperimentId,
                        beforeExposure = peak.IsUvExposed,
                        u = peak.u,
                        Pu = peak.Pu
                    })
                    .Where(r => r.Id == selectedID)
                    .ToList();

                dataGridView3.DataSource = pattersonPeaksData;
                dataGridView3.Columns["Id"].Visible = false;
            }
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView2.Columns[e.ColumnIndex].ValueType == typeof(double))
            {
                if (e.Value != null && double.TryParse(e.Value.ToString(), out double doubleValue))
                {
                    e.Value = doubleValue.ToString("0.###");
                    e.FormattingApplied = true;
                }
            }
        }

        private void dataGridView3_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView3.Columns[e.ColumnIndex].ValueType == typeof(double))
            {
                if (e.Value != null && double.TryParse(e.Value.ToString(), out double doubleValue))
                {
                    e.Value = doubleValue.ToString("0.###");
                    e.FormattingApplied = true;
                }
            }
        }

        private void DataBaseForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                Application.Exit();
            }
        }

        private void buildChartButton_Click(object sender, EventArgs e)
        {
            this.Hide();

            if(dataGridView1.SelectedRows.Count > 0)
            {
                Guid selectedID = (Guid)dataGridView1.SelectedRows[0].Cells["ID"].Value;
                Sample sample = new Sample();
                sample.experiment = dBContext.Experiments
                    .Include(exp => exp.Element)
                    .FirstOrDefault(exp => exp.Id == selectedID);
                sample.pattersonPeaks = dBContext.PattersonPeaks
                    .Where(peak => peak.ExperimentId == selectedID)
                    .ToList();
                sample.peaksData = dBContext.PeaksData
                    .Where(peak => peak.ExperimentId == selectedID)
                    .ToList();
                ChartForm.GetInstance().Show();
                ChartForm.GetInstance().PlotChartFromDataBase(sample);
            }    
            else
            {
                MessageBox.Show("No row selected");
            }
        }

        private void newExperimentButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
