using Dapper;
using DTE.CORE.Helpers;
using DTE.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.CORE.Cores
{
    public class MSSQLDAL : IDTE
    {
        private readonly ConnectionFactory _connectionFactory;

        public MSSQLDAL(string connection_string)
        {
            _connectionFactory = new ConnectionFactory(SupportedConnectionsTypes.MSSQL, connection_string);
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
            return _connectionFactory.CreateConnection().Query<string>($"USE [{database_name}]; SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{table_name}'");
        }

        public async Task<IEnumerable<string>> GetColumnsAsync(string database_name, string table_name)
        {
            return await _connectionFactory.CreateConnection().QueryAsync<string>($"USE [{database_name}]; SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{table_name}'");
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
            string query = $"USE [{database_name}]; SELECT TOP 1 * FROM  {table_name.GetTableName()}; ";
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
            string query = $"USE [{database_name}]; select * from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA+'.'+TABLE_NAME = '{table_name}';";

            return MSSQLHelper.ExecuteDataset(_connectionFactory.ConnectionString, query).Tables[0];
        }

        public async Task<DataTable> GetSchemaAsync(string database_name, string table_name)
        {
            string query = $"USE [{database_name}]; select * from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA+'.'+TABLE_NAME = '{table_name}';";

            return (await MSSQLHelper.ExecuteDatasetAsync(_connectionFactory.ConnectionString, query)).Tables[0];
        }

        public IEnumerable<string> GetTables(string database_name)
        {
            return _connectionFactory.CreateConnection().Query<string>($@"
            SELECT case 
                   when TABLE_TYPE = 'VIEW'
                        then TABLE_SCHEMA+'.'+TABLE_NAME +' (view)'
                        else TABLE_SCHEMA+'.'+TABLE_NAME
                   end 
                   FROM [{database_name}].INFORMATION_SCHEMA.TABLES");
        }

        public async Task<IEnumerable<string>> GetTablesAsync(string database_name)
        {
            var sql = $@"SELECT case
                    when TABLE_TYPE = 'VIEW'
                         then TABLE_SCHEMA + '.' + TABLE_NAME + ' (view)'
                         else TABLE_SCHEMA + '.' + TABLE_NAME
                    end FROM [{database_name}].INFORMATION_SCHEMA.TABLES";

            return await _connectionFactory.CreateConnection().QueryAsync<string>(sql);
        }
    }
    
}
