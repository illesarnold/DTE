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
    [Serializable]
    public class TreeViewModel : DataBindingBase45, ITreeViewModel
    {
        [XmlIgnore]
        public List<Exception> Errors = new List<Exception>();
        public TreeViewModel()
        {

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

        ConnectionModel connection;

        public ConnectionModel Connection
        {
            get
            {
                return connection;
            }

            set
            {
                connection = value;
                OnPropertyChanged();
            }
        }

        public async void LoadDatabasesAsync()
        {
            Databases.Clear();
            LoadConnection = true;
            Cores.ModelCore _modelCore = new Cores.ModelCore(connection);
            var databases = await Task.Run(() => _modelCore.DataBases());

            foreach (var dbName in databases)
            {
                var db = new Database
                {
                    DatabaseName = dbName,
                    ParentTreeBase = this,
                    Tables = new ObservableCollection<Table>()
                };

                var tables = await Task.Run(() => _modelCore.Tables(dbName));
                foreach (var tableName in tables)
                {
                    db.Tables.Add(new Table() { TableName = tableName, ParentTreeBase = this, DataBaseName = db.DatabaseName });
                }
                Databases.Add(db);
            }

            if (_modelCore.Errors.Count > 0)
            {
                Errors = _modelCore.Errors;
            }

            LoadConnection = false;

        }

        public ObservableCollection<Database> Databases { get; set; } = new ObservableCollection<Database>();
        [XmlIgnore]
        public TreeViewModel ParentTreeBase { get { return this; } set { ParentTreeBase = value; } }
    }
}
