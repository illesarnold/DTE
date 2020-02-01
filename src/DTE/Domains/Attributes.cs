using DTE.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DTE.Domains
{
    [Serializable]
    public class Attributes : DataBindingBase45, IAttributes
    {

        string _key = "Key";
        string _explicitKey = "ExplicitKey";
        string _table = "Table";
        string _write = "Write(false)";
        string _computed = "Computed";


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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
