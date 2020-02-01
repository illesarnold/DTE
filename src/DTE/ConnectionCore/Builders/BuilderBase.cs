using DTE.ConnectionCore;
using DTE.CORE;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DTE.Cores
{
    [Serializable]
    [XmlInclude(typeof(NpgSQLConnectionBuilder))]
    [XmlInclude(typeof(MSSQLConnectionBuilder))]
    [XmlInclude(typeof(MySQLConnectionBuilder))]
    public abstract class ConnectionBuilderBase : DataBindingBase45
    {
        private string _icon;
        private string connectionName = "ConnectionName";
        private string host = "localhost";
        private bool _isWindowsAuthentication;
        private string userName;
        private string password;
        private uint port = 3306;
        private uint _timeOut = 30;
        private string _database;

        public ConnectionBuilderBase()
        {

        }
        public ConnectionBuilderBase(string connection_string)
        {
            ConnectionString = connection_string;
        }

        public string Database
        {
            get
            {
                return _database;
            }

            set
            {
                _database = value;
                OnPropertyChanged();
            }
        }
        public Guid Id { get; set; } = Guid.NewGuid();
        private string _connectionString;
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                if (value != null && value != _connectionString)
                {
                    _connectionString = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Icon
        {
            get
            {
                return _icon;
            }

            set
            {
                _icon = value;
                OnPropertyChanged();
            }
        }
        public string ConnectionName
        {
            get
            {
                return connectionName;
            }

            set
            {
                connectionName = value;
                OnPropertyChanged();
            }
        }
        SupportedConnectionsTypes connectionType = SupportedConnectionsTypes.MySQL;
        public SupportedConnectionsTypes ConnectionType
        {
            get
            {
                return connectionType;
            }

            set
            {
                connectionType = value;
                switch (value)
                {
                    case SupportedConnectionsTypes.MySQL:
                        Icon = "Images/mysql.png";
                        break;
                    case SupportedConnectionsTypes.MSSQL:
                        Icon = "Images/mssql.png";
                        break;
                    case SupportedConnectionsTypes.PostgreSQL:
                        Icon = "Images/postgre.png";
                        break;
                    default:
                        break;
                }
                OnPropertyChanged();
            }
        }
        public string Host
        {
            get
            {
                return host;
            }

            set
            {
                host = value;
                OnPropertyChanged();
            }
        }


        public bool IsWindowsAuthentication
        {
            get { return _isWindowsAuthentication; }
            set { _isWindowsAuthentication = value; OnPropertyChanged(); }
        }

        public string UserName
        {
            get
            {
                return userName;
            }

            set
            {
                userName = value;
                OnPropertyChanged();
            }
        }
        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
                OnPropertyChanged();
            }
        }
        public uint Port
        {
            get
            {
                return port;
            }

            set
            {
                port = value;
                OnPropertyChanged();
            }
        }
        public uint TimeOut
        {
            get
            {
                return _timeOut;
            }

            set
            {
                _timeOut = value;
                OnPropertyChanged();
            }
        }


        public abstract void BuildConnectionString();
        public abstract void InitBuilder();

    }
}
