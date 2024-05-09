using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterson.viewmodel
{

    public class DataBaseFormViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler CreateNewExperimentRequested;

        public void OnCreateNewExperimentRequested()
        {
            CreateNewExperimentRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
