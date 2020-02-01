using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.CORE.Helpers
{
    public static class MSSQLHelper
    { 
        public static DataSet ExecuteDataset(string connectionString,string commandText)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = commandText;
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();

            conn.Open();
            da.Fill(ds);
            conn.Close();

            return ds;
        }
        public static async Task<DataSet> ExecuteDatasetAsync(string connectionString, string commandText)
        {
            DataSet ds = new DataSet();

            await Task.Run(() =>
            {
                SqlConnection conn = new SqlConnection(connectionString);
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = commandText;
                da.SelectCommand = cmd;

                conn.Open();
                da.Fill(ds);
                conn.Close();
            });

            return ds;
        }

        public static string GetTableName(this string tableName)
        {
            var table_info = tableName.Replace(" (view)","").Split('.');
            var schema = table_info.Count() > 1 ? table_info.First() : string.Empty;
            var table = table_info.Count() > 1 ? $"[{schema}].[{table_info.Last()}]" : table_info.Last();
            return table;
        }


    }
}
