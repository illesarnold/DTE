using DTE.ConnectionCore;
using DTE.CORE;
using DTE.Cores;
using DTE.Domains.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DTE.Domains
{
    [Serializable]
    public class TreeViewModel : DataBindingBase45, ITreeViewModel
    {
        public TreeViewModel()
        {

        }


        bool _loadConnection;
        [XmlIgnore]
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

        ConnectionBuilderBase _connectionBuilder;


        public ConnectionBuilderBase ConnectionBuilder
        {
            get
            {
                return _connectionBuilder;
            }

            set
            {
                _connectionBuilder = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadDatabasesAsync()
        {
            try
            {
                Databases.Clear();
                LoadConnection = true;
                var dteCore = new DTECore(_connectionBuilder.ConnectionType, _connectionBuilder.ConnectionString, new DTESettings().Settings);
                var databases = await dteCore.GetDatabasesAsync();

                foreach (var dbName in databases)
                {
                    var db = new Database
                    {
                        DatabaseName = dbName,
                        ParentTreeBase = this,
                        Tables = new ObservableCollection<Table>()
                    };

                    var tables = await dteCore.GetTablesAsync(dbName);
                    foreach (var tableName in tables)
                    {
                        db.Tables.Add(new Table() { TableName = tableName, ParentTreeBase = this, DataBaseName = db.DatabaseName });
                    }

                    Databases.Add(db);
                }
                LoadConnection = false;
            }
            catch (Exception ex)
            {
                LoadConnection = false;
                throw ex;
            }
        }

        [XmlIgnore]
        public ObservableCollection<Database> Databases { get; set; } = new ObservableCollection<Database>();
        [XmlIgnore]
        public TreeViewModel ParentTreeBase { get { return this; } set { ParentTreeBase = value; } }
    }
}
