using Patterson.model;
using Patterson.service;
using Patterson.service.implementation;
using Patterson.viewmodel;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Patterson
{
    public partial class Form1 : Form
    {
        private static Form1 instance;

        private readonly MainFormViewModel viewModel;
        private ChartForm chartForm = ChartForm.GetInstance();
        private DataBaseForm dataBaseForm = DataBaseForm.GetInstance();


        private Point selectionStart;
        private Point selectionEnd;
        private bool isSelecting;
        public Form1()
        {
            InitializeComponent();
            viewModel = new MainFormViewModel();
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            viewModel.ViewDataRequested += ViewModel_ViewDataRequested;
            dataBaseForm.ViewModel.CreateNewExperimentRequested += ViewModel_CreateNewExperimentRequested;
            chartForm.ViewModel.CreateNewExperimentRequested += ViewModel_CreateNewExperimentRequested;

            comboBox1.DataSource = viewModel.ComboBoxItems;
            comboBox1.DataBindings.Add("SelectedItem", viewModel, "SelectedElement", true, DataSourceUpdateMode.OnPropertyChanged);

            textBox1.DataBindings.Add("Text", viewModel, "MinTheta", false, DataSourceUpdateMode.OnPropertyChanged);
            textBox2.DataBindings.Add("Text", viewModel, "MaxTheta", false, DataSourceUpdateMode.OnPropertyChanged);
            textBox3.DataBindings.Add("Text", viewModel, "Lambda1", false, DataSourceUpdateMode.OnPropertyChanged);
            textBox4.DataBindings.Add("Text", viewModel, "Lambda2", false, DataSourceUpdateMode.OnPropertyChanged);
            textBox5.DataBindings.Add("Text", viewModel, "ExperimentDescription", true, DataSourceUpdateMode.OnPropertyChanged);


        }

        private void ViewModel_CreateNewExperimentRequested(object sender, EventArgs e)
        {
            this.Show();
            ResetForm();
            UnlockParametersChange();
        }

        public static Form1 GetInstance()
        {
            if (instance == null)
            {
                instance = new Form1();
            }
            return instance;
        }

        private void ViewModel_ViewDataRequested(object sender, EventArgs e)
        {
            this.Hide();
            dataBaseForm.Show();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ValidationErrorMessage" && !string.IsNullOrEmpty(viewModel.ValidationErrorMessage))
            {
                MessageBox.Show(viewModel.ValidationErrorMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.PropertyName == nameof(viewModel.ExecuteButtonVisible))
            {
                execute.Visible = viewModel.ExecuteButtonVisible;
            }
        }

        private void uploadPictureButton_Click(object sender, EventArgs e)
        {
            viewModel.UploadPicture(pictureBox);
        }


        private void executeButton_Click(object sender, EventArgs e)
        {
            Sample sample = viewModel.ExecutePattersonMethod(selectionStart, selectionEnd);

            if (sample != null)
            {
                chartForm.Run(sample, this);
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
        }

        public void ResetForm()
        {
            pictureBox.Image = null;
            comboBox1.SelectedIndex = -1;
            checkBox1.Checked = false;
            textBox4.Visible = false;
            execute.Visible = false;
            textBox1.Text = String.Empty;
            textBox2.Text = String.Empty;
            textBox3.Text = String.Empty;
            textBox4.Text = String.Empty;
            textBox5.Visible = true;
            textBox5.Text = String.Empty;
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
            this.Show();
            pictureBox.Image = null;
            textBox5.Visible = false;
            comboBox1.Visible = false;
            label3.Visible = false;
            execute.Visible = false;
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            selectionStart = e.Location;
            isSelecting = true;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectionEnd = e.Location;
                pictureBox.Invalidate();
            }
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (isSelecting)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(32, Color.Blue)))
                {
                    Rectangle selectionRect = GetNormalizedRectangle(selectionStart, selectionEnd);
                    e.Graphics.FillRectangle(brush, selectionRect);
                    e.Graphics.DrawRectangle(Pens.Blue, selectionRect);
                }
            }
            else if (!isSelecting && !selectionStart.IsEmpty && !selectionEnd.IsEmpty)
            {
                Rectangle selectionRect = GetNormalizedRectangle(selectionStart, selectionEnd);
                e.Graphics.DrawRectangle(Pens.Blue, selectionRect);
            }
        }

        private Rectangle GetNormalizedRectangle(Point p1, Point p2)
        {
            int x = Math.Min(p1.X, p2.X);
            int y = Math.Min(p1.Y, p2.Y);
            int width = Math.Abs(p1.X - p2.X);
            int height = Math.Abs(p1.Y - p2.Y);
            return new Rectangle(x, y, width, height);
        }



        private void button1_Click(object sender, EventArgs e)
        {
            Point start = new Point(selectionStart.X, selectionStart.Y);
            Point end = new Point(selectionEnd.X, selectionEnd.Y);
            selectionStart = Point.Empty;
            selectionEnd = Point.Empty;
            pictureBox.Image = viewModel.test(start, end);
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            isSelecting = false;
            pictureBox.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            viewModel.OnViewDataRequested();
        }
    }
}
