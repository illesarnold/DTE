using DTE.CORE.Interfaces;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace DTE.Domains
{
    [Serializable]
    public class Settings : DataBindingBase45, ISettings
    {
        string _accentName;
        Attributes _attributes = new Attributes();
        List<TypeConverter> _types = new List<TypeConverter>();
        bool _nullable;
        bool _caseSensitivity;
        string _prefix;
        string _postfix;
        bool _comments;
        bool _dataAnnotations = true;
        bool _fullProp;
        bool _dataMember;
        private string fullPropTemplate;
        private string _propTemplate;
        private string _classTemplate;
        private string _filePrefix;
        private string _filePostfix;
        private string _accentColor;
        private SolidColorBrush _accentBrush;

        public string AccentName
        {
            get
            {
                return _accentName;
            }

            set
            {
                _accentName = value;
                var theme = ThemeManager.DetectAppStyle(Application.Current);
                var accent = ThemeManager.GetAccent(value ?? "Blue");
                ThemeManager.ChangeAppStyle(Application.Current, accent, theme?.Item1);
                AccentColor = accent.Resources["AccentColorBrush"].ToString();
                AccentBrush = accent.Resources["AccentColorBrush"] as SolidColorBrush;
                OnPropertyChanged();
            }
        }

      
        public Attributes Attributes
        {
            get
            {
                return _attributes;
            }

            set
            {
                if (value != null && value != _attributes) _attributes = value;
                OnPropertyChanged();
            }
        }
        public List<TypeConverter> Types
        {
            get
            {
                return _types;
            }

            set
            {
                if (value != null && value != _types) _types = value;
                OnPropertyChanged();
            }
        }
        public bool Nullable
        {
            get
            {
                return _nullable;
            }

            set
            {
                _nullable = value;
                OnPropertyChanged();
            }
        }
       
        public bool CaseSensitivity
        {
            get
            {
                return _caseSensitivity;
            }

            set
            {
                _caseSensitivity = value;
                OnPropertyChanged();
            }
        }
        public string Prefix
        {
            get
            {
                return _prefix;
            }

            set
            {
                _prefix = value;
                OnPropertyChanged();
            }
        }
        public string Postfix
        {
            get
            {
                return _postfix;
            }

            set
            {
                _postfix = value;
                OnPropertyChanged();
            }
        }
        public bool Comments
        {
            get
            {
                return _comments;
            }

            set
            {
                _comments = value;
                OnPropertyChanged();
            }
        }
        public bool DataAnnotations
        {
            get
            {
                return _dataAnnotations;
            }

            set
            {
                _dataAnnotations = value;
                OnPropertyChanged();
            }
        }
        public bool FullProp
        {
            get
            {
                return _fullProp;
            }

            set
            {
                _fullProp = value;
                OnPropertyChanged();
            }
        }
        public bool DataMember
        {
            get
            {
                return _dataMember;
            }

            set
            {
                _dataMember = value;
                OnPropertyChanged();
            }
        }
        public string FullPropTemplate { get => fullPropTemplate; set { fullPropTemplate = value; OnPropertyChanged(); } }
        public string PropTemplate { get => _propTemplate; set { _propTemplate = value; OnPropertyChanged(); } }
        public string ClassTemplate { get => _classTemplate; set { _classTemplate = value; OnPropertyChanged(); } }
        public string FilePrefix { get => _filePrefix; set { _filePrefix = value; OnPropertyChanged(); } }
        public string FilePostfix { get => _filePostfix; set { _filePostfix = value; OnPropertyChanged(); } }


        [XmlIgnore]
        public string AccentColor
        {
            get
            {
                return _accentColor;
            }
            set
            {
                _accentColor = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public SolidColorBrush AccentBrush { get => _accentBrush; set { _accentBrush = value; OnPropertyChanged(); } }
        [XmlIgnore]
        IAttributes ISettings.Attributes { get => Attributes; }
        [XmlIgnore]
        List<ITypeConverter> ISettings.Types { get => new List<ITypeConverter>(Types); }
    }
}
