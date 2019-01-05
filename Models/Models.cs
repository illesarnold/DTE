using MahApps.Metro;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace DTE.Models
{
    public enum ConnectionTypes
    {
        MySQL, SQL_Server, SQL_CE, SQLite, Firebird, Oracle, PostgreSQL
    }
    [Serializable]
    public class ConnectionModel : DataBindingBase45
    {
        public ConnectionModel()
        {

        }
        public ConnectionModel(ConnectionTypes type, string connstring)
        {
            ConnType = type;
            ConnString = connstring;
            switch (type)
            {
                case ConnectionTypes.MySQL:
                    MySqlConnectionStringBuilder mybuilder = new MySqlConnectionStringBuilder(connstring);

                    Host = mybuilder.Server;
                    UserName = mybuilder.UserID;
                    Password = mybuilder.Password;
                    Port = (int)mybuilder.Port;
                    _timeOut = mybuilder.ConnectionTimeout;
                    ConnString = mybuilder.ConnectionString;

                    break;
                case ConnectionTypes.SQL_Server:
                    SqlConnectionStringBuilder sqlbuilder = new SqlConnectionStringBuilder(connstring);
                    var HostAndPort = sqlbuilder.DataSource.Split(',');
                    Host = HostAndPort[0];
                    UserName = sqlbuilder.UserID;
                    Password = sqlbuilder.Password;
                    if (HostAndPort.Count() > 1)
                        Port = int.Parse(HostAndPort[2]);

                    _timeOut = (uint)sqlbuilder.ConnectTimeout;

                    ConnString = sqlbuilder.ConnectionString;
                    break;
                case ConnectionTypes.SQL_CE:
                    MySqlConnectionStringBuilder sqlcebuilder = new MySqlConnectionStringBuilder(connstring);

                    Host = sqlcebuilder.Server;
                    UserName = sqlcebuilder.UserID;
                    Password = sqlcebuilder.Password;
                    Port = (int)sqlcebuilder.Port;
                    _timeOut = sqlcebuilder.ConnectionTimeout;
                    ConnString = sqlcebuilder.ConnectionString;

                    break;
                case ConnectionTypes.SQLite:
                    break;
                case ConnectionTypes.Firebird:
                    break;
                case ConnectionTypes.Oracle:
                    break;
                case ConnectionTypes.PostgreSQL:
                    NpgsqlConnectionStringBuilder npgsqlcebuilder = new NpgsqlConnectionStringBuilder(connstring);

                    Host = npgsqlcebuilder.Host;
                    UserName = npgsqlcebuilder.Username;
                    Password = npgsqlcebuilder.Password;
                    Port = (int)npgsqlcebuilder.Port;
                    _timeOut = (uint)npgsqlcebuilder.Timeout;
                    ConnString = npgsqlcebuilder.ConnectionString;

                    break;
                default:
                    break;
            }
        }

        string _connString;
        public string ConnString
        {
            get
            {
                return _connString;
            }

            set
            {
                _connString = value;
                OnPropertyChanged();
            }
        }

        private uint _timeOut = 500;
        public string Icon { get; set; }

        string connectionName = "ConName";
        public string ConnectionName
        {
            get
            {
                return connectionName;
            }

            set
            {
                if (value != null || value != connectionName) connectionName = value;
                OnPropertyChanged();
            }
        }
        ConnectionTypes connType = ConnectionTypes.MySQL;

        public ConnectionTypes ConnType
        {
            get
            {
                return connType;
            }

            set
            {
                if (value != connType) connType = value;
                switch (value)
                {
                    case ConnectionTypes.MySQL:
                        Icon = "Images/mysql.png";
                        break;
                    case ConnectionTypes.SQL_Server:
                        Icon = "Images/mssql.png";
                        break;
                    case ConnectionTypes.SQL_CE:
                        Icon = "Images/mssql.png";
                        break;
                    case ConnectionTypes.SQLite:
                        break;
                    case ConnectionTypes.Firebird:
                        break;
                    case ConnectionTypes.Oracle:
                        break;
                    case ConnectionTypes.PostgreSQL:
                        Icon = "Images/postgre.png";
                        break;
                    default:
                        break;
                }
                OnPropertyChanged();
            }
        }
        string host = "localhost";

        public string Host
        {
            get
            {
                return host;
            }

            set
            {
                if (value != null || value != host) host = value;
                OnPropertyChanged();
            }
        }
        string userName;

        public string UserName
        {
            get
            {
                return userName;
            }

            set
            {
                if (value != null || value != userName) userName = value;
                OnPropertyChanged();
            }
        }
        string password;

        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                if (value != null || value != password) password = value;
                OnPropertyChanged();
            }
        }
        int port = 3306;

        public int Port
        {
            get
            {
                return port;
            }

            set
            {
                if (value != port) port = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Create connectionstring to ConnString property
        /// </summary>
        public void BuilderBuildConnection()
        {
            switch (ConnType)
            {
                case ConnectionTypes.MySQL:
                    var mybuilder = new MySqlConnectionStringBuilder()
                    {
                        UserID = UserName,
                        Password = Password,
                        Server = Host,
                        Port = (uint)Port,
                        ConnectionTimeout = _timeOut
                    };
                    ConnString = mybuilder.ConnectionString;
                    break;
                case ConnectionTypes.SQL_Server:
                    var sqlBuilder = new SqlConnectionStringBuilder()
                    {
                        UserID = UserName,
                        Password = Password,
                        DataSource = Host,

                        ConnectTimeout = (int)_timeOut
                    };
                    if (Port > 0)
                        sqlBuilder.DataSource += $",{Port}";

                    ConnString = sqlBuilder.ConnectionString;
                    break;
                case ConnectionTypes.SQL_CE:
                    sqlBuilder = new SqlConnectionStringBuilder()
                    {
                        UserID = UserName,
                        Password = Password,
                        DataSource = Host,

                        ConnectTimeout = (int)_timeOut
                    };
                    if (Port > 0)
                        sqlBuilder.DataSource += $",{Port}";

                    ConnString = sqlBuilder.ConnectionString;
                    break;
                case ConnectionTypes.SQLite:
                    break;
                case ConnectionTypes.Firebird:
                    break;
                case ConnectionTypes.Oracle:
                    break;
                case ConnectionTypes.PostgreSQL:
                    var npgbuilder = new NpgsqlConnectionStringBuilder()
                    {
                        Username = UserName,
                        Password = Password,
                        Host = Host,
                        Port = Port,
                        Timeout = (int)_timeOut
                    };
                    ConnString = npgbuilder.ConnectionString;
                    break;
                default:
                    break;
            }
        }

        [XmlIgnore]
        public SqlConnectionStringBuilder MsSqlStringBuilder
        {
            get
            {
                var sqlBuilder =
                new SqlConnectionStringBuilder()
                {
                    UserID = UserName,
                    Password = Password,
                    DataSource = Host,

                    ConnectTimeout = (int)_timeOut
                };
                if (Port > 0)
                    sqlBuilder.DataSource += $",{Port}";

                return sqlBuilder;
            }
        }
        [XmlIgnore]
        public MySqlConnectionStringBuilder MySqlStringBuilder
        {
            get
            {
                return new MySqlConnectionStringBuilder()
                {
                    UserID = UserName,
                    Password = Password,
                    Server = Host,
                    Port = (uint)Port,
                    ConnectionTimeout = _timeOut
                };
            }
        }

    }
    public class ConnectionType
    {
        public ConnectionType()
        {

        }
        public ConnectionType(string name, ConnectionTypes type)
        {
            Name = name;
            ConnType = type;
        }

        public string Name { get; set; }
        public ConnectionTypes ConnType { get; set; }
    }
    public interface ITreeViewModel
    {
        TreeViewModel ParentTreeBase { get; set; }
    }
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
            Cores.ConnectionCore cc = new Cores.ConnectionCore(connection);
            var databases = await Task.Run(() => cc.DataBases());

            foreach (var dbName in databases)
            {
                var db = new Database
                {
                    DatabaseName = dbName,
                    ParentTreeBase = this,
                    Tables = new ObservableCollection<Table>()
                };

                var tables = await Task.Run(() => cc.Tables(dbName));
                foreach (var tableName in tables)
                {
                    db.Tables.Add(new Table() { TableName = tableName, ParentTreeBase = this, DataBaseName = db.DatabaseName });
                }
                Databases.Add(db);
            }

            if (cc.Errors.Count > 0)
            {
                Errors = cc.Errors;
            }

            LoadConnection = false;

        }

        public ObservableCollection<Database> Databases { get; set; } = new ObservableCollection<Database>();
        [XmlIgnore]
        public TreeViewModel ParentTreeBase { get { return this; } set { ParentTreeBase = value; } }
    }
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

            Cores.ConnectionCore cc = new Cores.ConnectionCore(ParentTreeBase.Connection);

            Tables.Clear();
            var tables = await Task.Run(() => cc.Tables(DatabaseName));

            foreach (var tableName in tables)
            {
                Tables.Add(new Table() { TableName = tableName, ParentTreeBase = ParentTreeBase, DataBaseName = DatabaseName });
            }

            if (cc.Errors.Count > 0)
            {
                Errors = cc.Errors;
            }

            LoadConnection = false;

        }
    }

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
                if (value != null || value != _dataBaseName) _dataBaseName = value;
                OnPropertyChanged();
            }
        }

    }

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
                if (value != null || value != _attributes) _attributes = value;
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
                if (value != null || value != _types) _types = value;
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
    }



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
                if (value != null || value != _key) _key = value;
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
                if (value != null || value != _explicitKey) _explicitKey = value;
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
                if (value != null || value != _table) _table = value;
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
                if (value != null || value != _write) _write = value;
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
                if (value != null || value != _computed) _computed = value;
                OnPropertyChanged();
            }
        }
    }

    [Serializable]
    public class TypeConverter : DataBindingBase45
    {
        public TypeConverter()
        {

        }
        public TypeConverter(string cType, string pType)
        {
            _cType = cType;
            _pType = pType;
        }

        string _cType;

        public string CType
        {
            get
            {
                return _cType;
            }

            set
            {
                _cType = value;
                OnPropertyChanged();
            }
        }
        string _pType;

        public string PType
        {
            get
            {
                return _pType;
            }

            set
            {
                _pType = value;
                OnPropertyChanged();
            }
        }

        public List<TypeConverter> GetDefaultTypes()
        {
            return new List<TypeConverter>()
            {
                new TypeConverter("System.Byte","byte"),
                new TypeConverter("System.SByte","sbyte"),
                new TypeConverter("System.Int32","int"),
                new TypeConverter("System.UInt32","uint"),
                new TypeConverter("System.Int16","short"),
                new TypeConverter("System.UInt16","ushort"),
                new TypeConverter("System.Int64","long"),
                new TypeConverter("System.UInt64","ulong"),
                new TypeConverter("System.Single","float"),
                new TypeConverter("System.Double","double"),
                new TypeConverter("System.Char","char"),
                new TypeConverter("System.Boolean","bool"),
                new TypeConverter("System.Object","object"),
                new TypeConverter("System.String","string"),
                new TypeConverter("System.Decimal","decimal"),
                new TypeConverter("System.DateTime","DateTime")
            };
        }
    }

}
