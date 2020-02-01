using DTE.CORE;
using DTE.Cores;
using MySql.Data.MySqlClient;
using Npgsql;
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
    public class NpgSQLConnectionBuilder : ConnectionBuilderBase, IConnectionBuilder
    {
        public NpgSQLConnectionBuilder()
        {

        }
        public NpgSQLConnectionBuilder(string connection_string) : base(connection_string)
        {
        }

        public override void BuildConnectionString()
        {
            NpgsqlConnectionStringBuilder npgsqlcebuilder = new NpgsqlConnectionStringBuilder(ConnectionString);
            npgsqlcebuilder.Host = Host;
            npgsqlcebuilder.Username = IsWindowsAuthentication ? string.Empty : UserName;
            npgsqlcebuilder.Password = IsWindowsAuthentication ? string.Empty : Password;
            npgsqlcebuilder.Database = Database;
            npgsqlcebuilder.Port = (int)Port;
            npgsqlcebuilder.Timeout = (int)TimeOut;
            npgsqlcebuilder.IntegratedSecurity = IsWindowsAuthentication;
            ConnectionString = npgsqlcebuilder.ConnectionString;

        }
        public override void InitBuilder()
        {
            NpgsqlConnectionStringBuilder mybuilder = new NpgsqlConnectionStringBuilder(ConnectionString ?? "");
            Host = mybuilder.Host;
            UserName = mybuilder.Username;
            Password = mybuilder.Password;
            Database = mybuilder.Database;
            Port = (uint)mybuilder.Port;
            TimeOut = (uint)mybuilder.Timeout;
            IsWindowsAuthentication = mybuilder.IntegratedSecurity;
        }
    }

}
