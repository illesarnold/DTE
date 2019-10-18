using DTE.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DTE.Models
{
    public class Database : DataBindingBase45, ITreeViewModel
    {
        [XmlIgnore]
        public List<Exception> Errors = new List<Exception>();

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
                foreach (var table in Tables)
                {
                    table.Checked = _checked;
                }
                OnPropertyChanged();
            }
        }

        string _databaseName;
        public string DatabaseName
        {
            get
            {
                return _databaseName;
            }

            set
            {
                _databaseName = value;
                OnPropertyChanged();
            }
        }

        bool _loadConnection;
        public bool LoadConnection
        {
            get
            {
                return _loadConnection;
            }

            set
            {
                _loadConnection = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<Table> _tables;

        public ObservableCollection<Table> Tables
        {
            get
            {
                return _tables;
            }

            set
            {
                _tables = value;
                OnPropertyChanged();
            }
        }

        public TreeViewModel ParentTreeBase { get; set; }

        public async void LoadTablesAsync()
        {
            LoadConnection = true;

            Cores.ModelCore _modelCore = new Cores.ModelCore(ParentTreeBase.Connection);

            Tables.Clear();
            var tables = await Task.Run(() => _modelCore.Tables(DatabaseName));

            foreach (var tableName in tables)
            {
                Tables.Add(new Table() { TableName = tableName, ParentTreeBase = ParentTreeBase, DataBaseName = DatabaseName });
            }

            if (_modelCore.Errors.Count > 0)
            {
                Errors = _modelCore.Errors;
            }

            LoadConnection = false;

        }
    }
}
