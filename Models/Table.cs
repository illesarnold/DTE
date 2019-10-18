using DTE.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.Models
{
    public class Table : DataBindingBase45, ITreeViewModel
    {
        bool _checked;

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
        public string TableName { get; set; }
        public TreeViewModel ParentTreeBase { get; set; }

        string _dataBaseName;

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
