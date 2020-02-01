using DTE.Cores;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DTE.ViewModels
{
    public enum TemplateType
    {
        Property,FullProperty,Class
    }
    public class TemplateVM : DataBindingBase45
    {

        List<string> PropTemplateLabels = new List<string>() { "[PrivateName]", "[PublicName]", "[Comment]", "[Type]", "[Annotations]" };
        List<string> ClassTemplateLabels = new List<string>() { "[Prefix]", "[Postfix]", "[Name]","[Properties]" };

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
                switch (_templateType)
                {
                    case TemplateType.Property:
                        Settings.Settings.PropTemplate = Text;
                        break;
                    case TemplateType.FullProperty:
                        Settings.Settings.FullPropTemplate = Text;
                        break;
                    case TemplateType.Class:
                        Settings.Settings.ClassTemplate = Text;
                        break;
                    default:
                        break;
                }
                Settings.SettingsSerialize();
                (_ as Window).Close();
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  \r\nStackTrace: {ex.StackTrace}");
            }

        }
        public TemplateVM(TemplateType templateType = TemplateType.FullProperty)
        {
            _templateType = templateType;
            switch (templateType)
            {
                case TemplateType.Property:
                    Text = Settings.Settings.PropTemplate;
                    Labels = PropTemplateLabels;
                    break;
                case TemplateType.FullProperty:
                    Text = Settings.Settings.FullPropTemplate;
                    Labels = PropTemplateLabels;
                    break;
                case TemplateType.Class:
                    Text = Settings.Settings.ClassTemplate;
                    Labels = ClassTemplateLabels;
                    break;
                default:
                    break;
            }
        }

        public TemplateVM(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;
        }

        DTESettings _settings = new DTESettings();
        private List<string> labels;
        private string text;
        private TemplateType _templateType;

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

        public List<string> Labels { get => labels; set { labels = value; OnPropertyChanged(); } }

        public string Text { get => text; set { text = value; OnPropertyChanged(); } }


    }
}
