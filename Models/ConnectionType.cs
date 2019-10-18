using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.Models
{
    public enum ConnectionTypes
    {
        MySQL, SQL_Server, SQL_CE, SQLite, Firebird, Oracle, PostgreSQL
    }



    public class ConnectionType
    {
        public ConnectionType()
        {

        }
        public ConnectionType(string name, ConnectionTypes type)
        {
            Name = name;
            ConnType = type;
        }

        public string Name { get; set; }
        public ConnectionTypes ConnType { get; set; }
    }
}
