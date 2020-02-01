using DTE.CORE.Interfaces;
using DTE.CORE.Cores;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTE.CORE
{
    internal class DteCoreFactory
    {
        private DteCoreFactory()
        {

        }


        public static IDTE CreateDTECore(SupportedConnectionsTypes type, string connection_string)
        {
            switch (type)
            {
                case SupportedConnectionsTypes.MariaDB:
                    break;
                case SupportedConnectionsTypes.MySQL:
                    return new MySQLDAL(connection_string);
                case SupportedConnectionsTypes.MSSQL:
                    return new MSSQLDAL(connection_string);
                case SupportedConnectionsTypes.PostgreSQL:
                    break;
                default:
                    break;
            }      
            return null;
        }
    }
}
