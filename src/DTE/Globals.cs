using DTE.CORE;
using DTE.Cores;
using DTE.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE
{
    public class Globals
    {
        public static List<KeyValuePair<SupportedConnectionsTypes,string>> ConnTypes = new List<KeyValuePair<SupportedConnectionsTypes, string>>()
        {
            new KeyValuePair<SupportedConnectionsTypes,string>(SupportedConnectionsTypes.MySQL,"MySql"),
            new KeyValuePair<SupportedConnectionsTypes,string>(SupportedConnectionsTypes.MSSQL,"MsSql"),
            new KeyValuePair<SupportedConnectionsTypes,string>(SupportedConnectionsTypes.PostgreSQL,"Postgre SQL")                   
        };
        public static DTECore CreateCoreByConnection(ConnectionBuilderBase builder)
        {
            return new DTECore(builder.ConnectionType, builder.ConnectionString, new DTESettings().Settings);
        }
    }
}
