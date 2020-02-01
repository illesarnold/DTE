using DTE.CORE;
using DTE.Cores;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DTE.ConnectionCore
{
    [Serializable]
    public class MSSQLConnectionBuilder : ConnectionBuilderBase, IConnectionBuilder
    {
        public MSSQLConnectionBuilder()
        {
            SetDefaults();
        }
        public MSSQLConnectionBuilder(string connection_string) : base(connection_string)
        {
            ConnectionType = SupportedConnectionsTypes.MSSQL;
            if (string.IsNullOrEmpty(connection_string))
                SetDefaults();
            else
                InitBuilder();
        }

        private void SetDefaults()
        {
            Port = 1433;
            ConnectionName = "MSSQLConnection";
            ConnectionType = SupportedConnectionsTypes.MSSQL;
        }

        public override void BuildConnectionString()
        {
            SqlConnectionStringBuilder sqlbuilder = new SqlConnectionStringBuilder(ConnectionString);
            sqlbuilder.DataSource = Host;
            sqlbuilder.DataSource += Port > 0 ? "" : "," + Port;
            sqlbuilder.IntegratedSecurity = IsWindowsAuthentication;
            sqlbuilder.UserID =  UserName ?? string.Empty;
            sqlbuilder.Password =  Password ?? string.Empty;
            sqlbuilder.InitialCatalog = Database ?? "";
            sqlbuilder.ConnectTimeout = (int)sqlbuilder.ConnectTimeout;
            ConnectionString = sqlbuilder.ConnectionString;
        }
        public override void InitBuilder()
        {
            SqlConnectionStringBuilder mybuilder = new SqlConnectionStringBuilder(ConnectionString ?? "");
            Host = mybuilder.DataSource.Split(',')[0];
            UserName = mybuilder.UserID;
            Password = mybuilder.Password;
            Database = mybuilder.InitialCatalog;
            Port = mybuilder.DataSource.Split(',').Count() < 2 ? (uint)1433 : Convert.ToUInt32(mybuilder.DataSource.Split(',')[1]);
            TimeOut = (uint)mybuilder.ConnectTimeout;
            IsWindowsAuthentication = mybuilder.IntegratedSecurity;
        }
    }
}
