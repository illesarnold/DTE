using MahApps.Metro.Controls.Dialogs;
using DTE.Cores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DTE.ViewModels
{
    public class DataAnnotationsVM : DataBindingBase45
    {
        private IDialogCoordinator dialogCoordinator;

        public RelayCommand Save
        {
            get
            {
                return new RelayCommand(_ => SaveSettings(_));
            }
        }
        private async void SaveSettings(object _)
        {
            try
            {
                Settings.SettingsSerialize();
                (_ as Window).Close();
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  /r/nStackTrace: {ex.StackTrace}");
            }

        }

        public DataAnnotationsVM()
        {

        }

     
        public DataAnnotationsVM(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;
        }

        SettingsCore _settings = new SettingsCore();
        
        public SettingsCore Settings
        {
            get
            {
                return _settings;
            }
        
            set
            {
                if (value != null || value != _settings) _settings = value;
        	 	OnPropertyChanged();
            }
        }


    }
}
