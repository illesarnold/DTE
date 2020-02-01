using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DTE.CORE.Interfaces
{
    public interface IDTE
    {
        IEnumerable<string> GetDatabases();
        IEnumerable<string> GetTables(string database_name);
        IEnumerable<string> GetColumns(string database_name,string table_name);
        DataTable ExecuteQueryWithTableInfo(string query);
        DataTable GetSchema(string database_name, string table_name);
        DataTable GetFirstRowWithSchemaInfo(string database_name, string table_name);
        Task<IEnumerable<string>> GetDatabasesAsync();
        Task<IEnumerable<string>> GetTablesAsync(string database_name);
        Task<IEnumerable<string>> GetColumnsAsync(string database_name, string table_name);
        Task<DataTable> GetSchemaAsync(string database_name, string table_name);

    }
}
