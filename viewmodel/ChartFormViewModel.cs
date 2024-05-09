using Patterson.model;
using System;
using System.ComponentModel;

namespace Patterson.viewmodel
{
    public class ChartFormViewModel : INotifyPropertyChanged
    {
        private static ChartFormViewModel instance;

        public event EventHandler ViewDataRequested;
        public event EventHandler CreateNewExperimentRequested;
        public event PropertyChangedEventHandler PropertyChanged;


        public void OnCreateNewExperimentRequested()
        {
            CreateNewExperimentRequested?.Invoke(this, EventArgs.Empty);
        }

        public void OnViewDataRequested()
        {
            ViewDataRequested?.Invoke(this, EventArgs.Empty);
        }




        public static ChartFormViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ChartFormViewModel();
                }
                return instance;
            }
        }

        public void ConductNewExperiment()
        {
            OnCreateNewExperimentRequested();
        }
    }
}
