using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DTE.Models
{
    [Serializable]
    public class Settings : DataBindingBase45
    {
        string _accenColor;

        public string AccenColor
        {
            get
            {
                return _accenColor;
            }

            set
            {
                _accenColor = value;
                var theme = ThemeManager.DetectAppStyle(Application.Current);
                var accent = ThemeManager.GetAccent(value ?? "Blue");
                ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
                OnPropertyChanged();
            }
        }

        Attributes _attributes = new Attributes();

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

        List<TypeConverter> _types = new List<TypeConverter>();
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

        bool _nullable;
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

        bool _static;
        public bool Static
        {
            get
            {
                return _static;
            }

            set
            {
                _static = value;
                OnPropertyChanged();
            }
        }

        bool _caseSensitivity;
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

        string _prefix;
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

        string _postfix;
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

        bool _comments;

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

        bool _dataAnnotations = true;

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

        string _cRUD_prefix;
        public string CRUD_prefix
        {
            get
            {
                return _cRUD_prefix;
            }

            set
            {
                if (value != null && value != _cRUD_prefix) _cRUD_prefix = value;
                OnPropertyChanged();
            }
        }

        string _cRUD_postfix;
        public string CRUD_postfix
        {
            get
            {
                return _cRUD_postfix;
            }

            set
            {
                if (value != null && value != _cRUD_postfix) _cRUD_postfix = value;
                OnPropertyChanged();
            }
        }


        bool _fullProp;
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

        bool _dataMember;

        private string fullPropTemplate;
        private string _propTemplate;
        private string _classTemplate;

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

    }
}
