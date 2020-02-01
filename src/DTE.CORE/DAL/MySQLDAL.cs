using Dapper;
using DTE.CORE.Helpers;
using DTE.CORE.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace DTE.CORE.Cores
{
    public class MySQLDAL : IDTE
    {
        private readonly ConnectionFactory _connectionFactory;
        public MySQLDAL(string connection_string)
        {
            _connectionFactory = new ConnectionFactory(SupportedConnectionsTypes.MySQL, connection_string);
        }
        public DataTable ExecuteQueryWithTableInfo(string query)
        {
            return DTEMySqlHelper.GetQuerySchema(_connectionFactory.ConnectionString, query);
        }
        public IEnumerable<string> GetColumns(string database_name, string table_name)
        {
            return _connectionFactory.CreateConnection().Query<string>($"SHOW COLUMNS FROM {database_name}.{table_name}");
        }
        public async Task<IEnumerable<string>> GetColumnsAsync(string database_name, string table_name)
        {
            return await _connectionFactory.CreateConnection().QueryAsync<string>($"SHOW COLUMNS FROM {database_name}.{table_name}");
        }
        public IEnumerable<string> GetDatabases()
        {
            return _connectionFactory.CreateConnection().Query<string>("SHOW DATABASES");
        }
        public async Task<IEnumerable<string>> GetDatabasesAsync()
        {
            return await _connectionFactory.CreateConnection().QueryAsync<string>("SHOW DATABASES");
        }
        public DataTable GetFirstRowWithSchemaInfo(string database_name, string table_name)
        {
            string query = $"SELECT * FROM {database_name}.{table_name} limit 1";

            return DTEMySqlHelper.GetQuerySchema(_connectionFactory.ConnectionString,query);
        }
        public DataTable GetSchema(string database_name, string table_name)
        {
            string query = $"DESCRIBE {database_name}.{table_name}";

            return MySqlHelper.ExecuteDataset(_connectionFactory.ConnectionString, query).Tables[0];
        }
        public async Task<DataTable> GetSchemaAsync(string database_name, string table_name)
        {
            string query = $"DESCRIBE {database_name}.{table_name}";

            return (await MySqlHelper.ExecuteDatasetAsync(_connectionFactory.ConnectionString, query)).Tables[0];
        }
        public IEnumerable<string> GetTables(string database_name)
        {
            return _connectionFactory.CreateConnection().Query<string>($"SHOW TABLES FROM {database_name};");
        }
        public async Task<IEnumerable<string>> GetTablesAsync(string database_name)
        {
            return await _connectionFactory.CreateConnection().QueryAsync<string>($"SHOW TABLES FROM {database_name};");
        }
    }
}
