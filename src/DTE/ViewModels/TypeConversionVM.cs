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
    public class TypeConversionVM : DataBindingBase45
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
                await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  \r\nStackTrace: {ex.StackTrace}");
            }

        }

        public TypeConversionVM()
        {

        }


        public TypeConversionVM(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;
        }

        DTESettings _settings = new DTESettings();

        public DTESettings Settings
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
