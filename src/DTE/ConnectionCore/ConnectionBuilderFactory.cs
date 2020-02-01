using DTE.ConnectionCore;
using DTE.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.Cores
{
    public class ConnectionBuilderFactory
    {
        private ConnectionBuilderFactory()
        {

        }

        public static IConnectionBuilder CreateConnectionBuilder(SupportedConnectionsTypes connectionType, string connectionString="")
        {
            switch (connectionType)
            {
                case SupportedConnectionsTypes.MariaDB:
                    break;
                case SupportedConnectionsTypes.MySQL:
                    return new MySQLConnectionBuilder(connectionString);
                case SupportedConnectionsTypes.MSSQL:
                    return new MSSQLConnectionBuilder(connectionString);
                case SupportedConnectionsTypes.PostgreSQL:
                    break;
            }

            return null;
        }
    }
}
