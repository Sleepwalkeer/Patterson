using Npgsql;
using Patterson.model;
using Patterson.utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Patterson
{
    public partial class ChartForm : Form
    {
        //NOT SURE IF NEEDED
        private Sample sample;
        private Form1 startForm;
        private bool isPostPicUploaded;

        public ChartForm()
        {
            InitializeComponent();
            PostInit();
        }

        public void PlotChart(Sample sample) {

            List<double> ps = sample.ps;


            Series series = new Series(sample.element + " pre Ps");
            if (isPostPicUploaded)
            {
                series = new Series(sample.element + " post Ps")
                {
                    Color = Color.Red
                };
            }
            series.ChartType = SeriesChartType.Line;

            for (int i = 0; i < ps.Count; i++)
            {
                series.Points.AddXY(i, ps[i]);
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

        public void Run(Sample sample, Form1 callbackForm) {
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

        private void button4_Click(object sender, EventArgs e)
        {
                string host = PropertyReader.GetProperty("Host");
                int port = int.Parse(PropertyReader.GetProperty("Port"));
                string database = PropertyReader.GetProperty("Database");
                string username = PropertyReader.GetProperty("Username");
                string password = PropertyReader.GetProperty("Password");

                string connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";

                try
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();

                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            MessageBox.Show("Connected to the database successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to connect to the database.");
                        }
                    }
                }
                catch (NpgsqlException ex)
                {
                    
                }
            }
        }
    }
