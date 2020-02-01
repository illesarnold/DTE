using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.CORE.Helpers
{
    public class DTEMySqlHelper
    {
        public static DataTable GetQuerySchema(string connection_string, string query)
        {
            MySqlConnection mySqlConnection = new MySqlConnection(connection_string);
            try
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);
                mySqlConnection.Open();
                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader(CommandBehavior.KeyInfo);
                DataTable dt = mySqlDataReader.GetSchemaTable();

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                mySqlConnection.Close();

            }
        }
    }
}
