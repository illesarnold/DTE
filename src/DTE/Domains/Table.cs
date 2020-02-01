using DTE.Domains.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.Domains
{
    public class Table : DataBindingBase45, ITreeViewModel
    {
        bool _checked;
        public string TableName { get; set; }
        public TreeViewModel ParentTreeBase { get; set; }
        string _dataBaseName;


        public bool Checked
        {
            get
            {
                return _checked;
            }

            set
            {
                _checked = value;
                OnPropertyChanged();
            }
        }
        public string DataBaseName
        {
            get
            {
                return _dataBaseName;
            }

            set
            {
                if (value != null && value != _dataBaseName) _dataBaseName = value;
                OnPropertyChanged();
            }
        }

    }
}
