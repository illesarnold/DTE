using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DTE.CORE
{
    internal class ConnectionFactory
    {
        public SupportedConnectionsTypes ConnectionType { get; set; } = SupportedConnectionsTypes.MySQL;
        public string ConnectionString { get; set; }
        public ConnectionFactory(SupportedConnectionsTypes connectionType, string connectionString)
        {
            ConnectionType = connectionType;
            ConnectionString = connectionString;
        }
       
        public IDbConnection CreateConnection()
        {
            switch (ConnectionType)
            {
                case SupportedConnectionsTypes.MariaDB:
                    break;
                case SupportedConnectionsTypes.MySQL:
                    return new MySqlConnection(ConnectionString);
                case SupportedConnectionsTypes.MSSQL:
                    return new SqlConnection(ConnectionString);
                case SupportedConnectionsTypes.PostgreSQL:
                    break;
            }
            return null;
        }

    
    }
}
