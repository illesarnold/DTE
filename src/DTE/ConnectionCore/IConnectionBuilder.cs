using DTE.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DTE.Cores
{
    public interface IConnectionBuilder
    {
        Guid Id { get; set; }
        string ConnectionString { get; set; }
        string ConnectionName { get; set; }
        SupportedConnectionsTypes ConnectionType { get; set; }
        string Host { get; set; }
        bool IsWindowsAuthentication { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string Database { get; set; }
        uint Port { get; set; }
        uint TimeOut { get; set; }
        void BuildConnectionString();
        void InitBuilder();
    }
}