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
                if (value != null && value != connectionName) connectionName = value;
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
                if (value != null && value != host) host = value;
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
                if (value != null && value != userName) userName = value;
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
                if (value != null && value != password) password = value;
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
}
