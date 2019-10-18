using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE
{
    public class Globals
    {
        public static Cores.ModelCore ModelCore;

        public static List<Models.ConnectionType> ConnTypes = new List<Models.ConnectionType>()
        {
            new Models.ConnectionType("MySql",Models.ConnectionTypes.MySQL),
            new Models.ConnectionType("MsSql Server",Models.ConnectionTypes.SQL_Server),
            new Models.ConnectionType("MsSql CE",Models.ConnectionTypes.SQL_CE),
            new Models.ConnectionType("Postgre SQL",Models.ConnectionTypes.PostgreSQL)
        };

    }
}
