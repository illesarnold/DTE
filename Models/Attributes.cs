using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.Models
{
    [Serializable]
    public class Attributes : DataBindingBase45
    {

        string _key = "Key";

        public string Key
        {
            get
            {
                return _key;
            }

            set
            {
                if (value != null && value != _key) _key = value;
                OnPropertyChanged();
            }
        }
        string _explicitKey = "ExplicitKey";

        public string ExplicitKey
        {
            get
            {
                return _explicitKey;
            }

            set
            {
                if (value != null && value != _explicitKey) _explicitKey = value;
                OnPropertyChanged();
            }
        }
        string _table = "Table";

        public string Table
        {
            get
            {
                return _table;
            }

            set
            {
                if (value != null && value != _table) _table = value;
                OnPropertyChanged();
            }
        }

        string _write = "Write(false)";

        public string Write
        {
            get
            {
                return _write;
            }

            set
            {
                if (value != null && value != _write) _write = value;
                OnPropertyChanged();
            }
        }
        string _computed = "Computed";

        public string Computed
        {
            get
            {
                return _computed;
            }

            set
            {
                if (value != null && value != _computed) _computed = value;
                OnPropertyChanged();
            }
        }
    }
}
