using DTE.CORE;
using DTE.Cores;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DTE.ConnectionCore
{
    [Serializable]
    public class MySQLConnectionBuilder : ConnectionBuilderBase, IConnectionBuilder
    {
        public MySQLConnectionBuilder()
        {
                SetDefaults();
        }
        public MySQLConnectionBuilder(string connection_string) : base(connection_string)
        {
            if (string.IsNullOrEmpty(connection_string))
                SetDefaults();
            else
                InitBuilder();
        }
        private void SetDefaults()
        {
            Port = 3306;
            ConnectionName = "MYSQLConnection";
            ConnectionType = SupportedConnectionsTypes.MySQL;
        }
        public override void BuildConnectionString()
        {
            MySqlConnectionStringBuilder mybuilder = new MySqlConnectionStringBuilder(ConnectionString ?? "");
            mybuilder.Server = Host;
            mybuilder.UserID = IsWindowsAuthentication ? string.Empty : UserName;
            mybuilder.Password = IsWindowsAuthentication ? string.Empty : Password;
            mybuilder.Database = Database;
            mybuilder.Port = (uint)Port;
            mybuilder.ConnectionTimeout = TimeOut;
            mybuilder.IntegratedSecurity = IsWindowsAuthentication;
            ConnectionString = mybuilder.ConnectionString;
        }


        public override void InitBuilder()
        {
            MySqlConnectionStringBuilder mybuilder = new MySqlConnectionStringBuilder(ConnectionString ?? "");
            Host=mybuilder.Server;
            UserName=mybuilder.UserID;
            Password=mybuilder.Password;
            Database=mybuilder.Database;
            Port = mybuilder.Port;
            TimeOut =mybuilder.ConnectionTimeout = TimeOut;
            IsWindowsAuthentication = mybuilder.IntegratedSecurity;
        }

       
    }
}
