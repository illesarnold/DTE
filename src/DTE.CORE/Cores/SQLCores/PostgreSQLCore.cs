using DTE.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DTE.CORE.Cores
{
    public class PostgreSQLCore : IDTE
    {
        private readonly ConnectionFactory _connectionFactory;
        public PostgreSQLCore(string connection_string)
        {
            _connectionFactory = new ConnectionFactory(SupportedConnectionsTypes.PostgreSQL, connection_string);
        }

        public DataTable ExecuteQueryWithTableInfo(string query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetColumns(string database_name, string table_name)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetColumnsAsync(string database_name, string table_name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetDatabases()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetDatabasesAsync()
        {
            throw new NotImplementedException();
        }

        public DataTable GetFirstRowWithSchemaInfo(string database_name, string table_name)
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchema(string database_name, string table_name)
        {
            throw new NotImplementedException();
        }

        public Task<DataTable> GetSchemaAsync(string database_name, string table_name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetTables(string database_name)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetTablesAsync(string database_name)
        {
            throw new NotImplementedException();
        }
    }
}
