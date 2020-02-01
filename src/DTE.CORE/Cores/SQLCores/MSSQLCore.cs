using Dapper;
using DTE.CORE.Helpers;
using DTE.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DTE.CORE.Cores
{
    public class MSSQLCore : IDTE
    {
        private readonly ConnectionFactory _connectionFactory;

        public MSSQLCore(string connection_string)
        {
            _connectionFactory = new ConnectionFactory(SupportedConnectionsTypes.MySQL, connection_string);
        }

        public DataTable ExecuteQueryWithTableInfo(string query)
        {
            SqlConnection sqlConnection = new SqlConnection(_connectionFactory.ConnectionString);
            SqlCommand SqlCommand = new SqlCommand(query, sqlConnection);
            sqlConnection.Open();
            SqlDataReader SqlDataAdapter = SqlCommand.ExecuteReader(CommandBehavior.KeyInfo);
            DataTable dt = SqlDataAdapter.GetSchemaTable() ?? new DataTable();
            sqlConnection.Close();

            return dt;
        }

        public IEnumerable<string> GetColumns(string database_name, string table_name)
        {
            return _connectionFactory.CreateConnection().Query<string>($"USE {database_name}; SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{table_name}'");
        }

        public async Task<IEnumerable<string>> GetColumnsAsync(string database_name, string table_name)
        {
            return await _connectionFactory.CreateConnection().QueryAsync<string>($"USE {database_name}; SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{table_name}'");
        }

        public IEnumerable<string> GetDatabases()
        {
            return _connectionFactory.CreateConnection().Query<string>("SELECT name FROM master.sys.databases");
        }

        public async Task<IEnumerable<string>> GetDatabasesAsync()
        {
            return await _connectionFactory.CreateConnection().QueryAsync<string>("SELECT name FROM master.sys.databases");
        }

        public DataTable GetFirstRowWithSchemaInfo(string database_name, string table_name)
        {
            string query = $"USE {database_name}; SELECT TOP 1 * FROM {database_name}; ";
            SqlConnection sqlConnection = new SqlConnection(_connectionFactory.ConnectionString);
            SqlCommand SqlCommand = new SqlCommand(query, sqlConnection);
            sqlConnection.Open();
            SqlDataReader SqlDataAdapter = SqlCommand.ExecuteReader(CommandBehavior.KeyInfo);
            DataTable dt = SqlDataAdapter.GetSchemaTable() ?? new DataTable();
            sqlConnection.Close();

            return dt;
        }

      
        public DataTable GetSchema(string database_name, string table_name)
        {
            string query = $"USE {database_name}; select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{table_name}';";

            return MSSQLHelper.ExecuteDataset(_connectionFactory.ConnectionString, query).Tables[0];
        }

        public async Task<DataTable> GetSchemaAsync(string database_name, string table_name)
        {
            string query = $"DESCRIBE {database_name}.{table_name}";

            return (await MSSQLHelper.ExecuteDatasetAsync(_connectionFactory.ConnectionString, query)).Tables[0];
        }

        public IEnumerable<string> GetTables(string database_name)
        {
            return _connectionFactory.CreateConnection().Query<string>($"SELECT Distinct TABLE_NAME FROM {database_name}.information_schema.TABLES;");
        }

        public async Task<IEnumerable<string>> GetTablesAsync(string database_name)
        {
            return await _connectionFactory.CreateConnection().QueryAsync<string>($"SELECT Distinct TABLE_NAME FROM {database_name}.information_schema.TABLES;");
        }
    }
    
}
