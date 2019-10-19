using Dapper;
using DTE.Models;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.Cores
{
    public class ModelCore
    {
        IDbConnection connection;
        ConnectionModel _model;
        Settings _settings;
        public List<Exception> Errors = new List<Exception>();
        public ModelCore(ConnectionModel model)
        {
            _model = model;
            SetConnection();
        }
        private void SetConnection()
        {
            try
            {
                switch (_model.ConnType)
                {
                    case ConnectionTypes.MySQL:
                        connection = new MySqlConnection(_model.ConnString);
                        break;
                    case ConnectionTypes.SQL_CE:
                        connection = new SqlConnection(_model.ConnString);
                        break;
                    case ConnectionTypes.PostgreSQL:
                        connection = new NpgsqlConnection(_model.ConnString);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Errors.Add(ex);
            }


        }


        // ------------------------------ Model Create ------------------------------ //
        public string CreateModels(List<Database> databases, Settings settings)
        {
            string finishedModels = "";
            _settings = settings;
            foreach (var database in databases)
            {
                if (database == null || database.Tables == null || database.Tables.Count == 0)
                    continue;

                var tables = database.Tables.Where(x => x.Checked);

                if (tables == null)
                    continue;

                foreach (var table in tables)
                {
                    finishedModels += CreateModel(database.DatabaseName, table.TableName) + "\r\n";
                }
            }


            return finishedModels;
        }
        public string CreateModel(string database, string table, Settings settings)
        {
            _settings = settings;

            return CreateModel(database, table);
        }
        private string CreateModel(string database, string tablename)
        {

            DataTable dt = GetFirstRowWithSchemaInfo(tablename, database);
            var table_describe = GetSchemaInfo(dt.TableName, database);

            return CreateModel(dt, table_describe);
        }
        public string CreateModel(string query, Settings settings)
        {
            _settings = settings;
            DataTable dt = ExecuteQuery(query);

            return CreateModel(dt);
        }

        private string CreateModel(DataTable dt, DataTable table_descibe = null)
        {

            var tablename = dt.TableName;


            var class_annotations = "";
            string properties = "";
            
            if (_settings.DataAnnotations)
                class_annotations += $"[{_settings.Attributes.Table}(\"{tablename}\")] \r\n";
            if (_settings.DataMember)
                class_annotations += $"\t[DataContract]\r\n";

            var class_name = ColumnNameToPropName(tablename).Replace("_", "");


           


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var auto_increment = bool.Parse(dt.Rows[i]["IsAutoIncrement"]?.ToString());
                var column_name = dt.Rows[i]["ColumnName"].ToString();
                var allow_null = bool.Parse(dt.Rows[i]["AllowDBNull"].ToString());
                var column_type = dt.Rows[i]["DataType"].ToString();
                var isKey = bool.Parse(dt.Rows[i]["IsKey"].ToString());

                var name = column_name;
                name = _settings.CaseSensitivity ? column_name : ColumnNameToPropName(column_name.ToString());
                var cType = GetCsharpType(column_type, allow_null);
                var comment = "";
                var annotations = "";



                if (_settings.Comments)
                {
                    for (int j = 0; j < table_descibe?.Columns.Count; j++)
                    {
                        comment += $"  //  {table_descibe.Columns[j].ColumnName}: {table_descibe?.Rows[i][j].ToString()} ";
                    }
                }

                if (isKey)
                {
                    if (_settings.DataAnnotations && auto_increment)
                        annotations += $"\r\n    [{_settings.Attributes.Key}]";
                    if (_settings.DataAnnotations && auto_increment == false)
                        annotations += $"    [{_settings.Attributes.ExplicitKey}]";
                }
                if (_settings.DataMember)
                    annotations += $"\r\n     [DataMember]";

                if (_settings.FullProp)
                {
                    properties = FullPropText(properties, column_name, name, comment, cType,annotations);

                }
                else
                {
                    properties = PropText(properties, column_name, name, comment, cType,annotations);
                }



            }





            return _settings.ClassTemplate?.Replace("[Annotations]", class_annotations)
                                          ?.Replace("[Prefix]", _settings.Prefix)
                                          ?.Replace("[Name]", class_name)
                                          ?.Replace("[Postfix]", _settings.Postfix)
                                          ?.Replace("[Properties]", properties);




        }

        private string PropText(string model, string column_name, string name, string comment, string cType, string annotations)
        {
            model += _settings.PropTemplate?.Replace("[Type]", cType)
                                        ?.Replace("[PrivateName]", column_name)
                                        ?.Replace("[PublicName]", name)
                                        ?.Replace("[Comment]", comment)
                                        ?.Replace("[Annotations]", annotations);
            return model;
        }

        private string FullPropText(string model, string column_name, string name, string comment, string cType, string annotations)
        {
            model += _settings.FullPropTemplate?.Replace("[Type]", cType)
                                          ?.Replace("[PrivateName]", column_name)
                                          ?.Replace("[PublicName]", name)
                                          ?.Replace("[Comment]", comment)
                                          ?.Replace("[Annotations]", annotations);
            return model;
        }




        // ------------------------------ Core Create ------------------------------ //



        public string CreateCores(List<Database> databases, Settings settings)
        {
            string finishedModels = "";
            _settings = settings;
            foreach (var database in databases)
            {
                if (database == null || database.Tables == null || database.Tables.Count == 0)
                    continue;

                var tables = database.Tables.Where(x => x.Checked);

                if (tables == null)
                    continue;

                foreach (var table in tables)
                {
                    finishedModels += CreateCore(database.DatabaseName, table.TableName) + "\r\n";
                }
            }


            return finishedModels;
        }
        public string CreateCore(string database, string table, Settings settings)
        {
            _settings = settings;

            return CreateCore(database, table);
        }
        private string CreateCore(string database, string tablename)
        {
            // DataTable dt = GetFirstRowWithSchemaInfo(tablename, database);
            string tableU = FirstCharToUpper(tablename);
            string ModelName = _settings.Prefix + ColumnNameToPropName(tablename).Replace("_", "") + _settings.Postfix;
            var staticString = "";
            var constructor = "";
            var classname = _settings.CRUD_prefix + tableU + _settings.CRUD_postfix;
            if (_settings.Static)
            {
                staticString = "static";
            }
            else
            {
                constructor = $@"
	private IDbConnection connection;
	public {classname}(IDbConnection _connection)
	{{
			this.connection = _connection;
	}}";
            }

            string result = $@"
public class {classname}
{{
	// USE Dapper and Dapper Contrib nugate
									
	{constructor}
	
	/// <summary>
	/// Get a single entity by ID.
	/// </summary>
	/// <returns>Entity</returns>
	public {staticString} {ModelName} Get_{tableU}(uint id)
	{{
		return connection.Get<{ModelName}>(id);
	}}
	public {staticString} List<{ModelName}> GetAll_{tableU}()
	{{
		return connection.GetAll<{ModelName}>().ToList();
	}}
	public {staticString} long Insert_{tableU}({ModelName} {tablename})
	{{
		return connection.Insert({tablename});
	}}
	public {staticString} long Insert_{tableU}(List<{ModelName}> {tablename})
	{{
		return connection.Insert({tablename});
	}}
	public {staticString} bool Update_{tableU}({ModelName} {tablename})
	{{
		return connection.Update({tablename});
	}}
	public {staticString} bool Update_{tableU}(List<{ModelName}> {tablename})
	{{
		return connection.Update({tablename});
	}}
	public {staticString} bool Delete_{tableU}({ModelName} {tablename})
	{{
		return connection.Delete({tablename});
	}}
	public {staticString} bool Delete_{tableU}(List<{ModelName}> {tablename})
	{{
		return connection.Delete({tablename});
	}}
}}";
            return result;
        }


        // ------------------------------ Methods ------------------------------ //

        public string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
        public static string ColumnNameToPropName(string colname)
        {
            var parts = colname.Split('_');
            var propName = "";
            if (parts.Count() != 0)
            {
                int count = 0;
                foreach (var item in parts)
                {
                    if (item != string.Empty)
                    {
                        if (count == parts.Count() - 1)
                        {
                            propName += item.First().ToString().ToUpper() + item.Substring(1);
                        }
                        else
                        {
                            propName += item.First().ToString().ToUpper() + item.Substring(1) + "_";
                        }
                    }



                    count++;
                }
            }
            else
            {
                propName += colname.First().ToString().ToUpper() + colname.Substring(1);
            }

            return propName;

        }


        #region Database type to c#
        private string GetCsharpType(string type, bool nullable)
        {
            string backType = "";

            backType = _settings.Types.FirstOrDefault(x => x.CType == type)?.PType ?? "";
            if (nullable && _settings.Nullable == true)
            {
                if (backType != "string" && backType != "char")
                {
                    backType += "?";
                }
            }

            return backType;
        }

        #endregion
        // ------------------------------ Database info ------------------------------ //

        public DataTable GetFirstRowWithSchemaInfo(string tablename, string database)
        {
            DataTable dt = new DataTable();

            try
            {
                if (_model.ConnType == ConnectionTypes.MySQL)
                {
                    string query = $"SELECT * FROM {database}.{tablename} limit 1";

                    MySqlConnection mySqlConnection = new MySqlConnection(_model.ConnString);
                    MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);
                    mySqlConnection.Open();
                    MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader(CommandBehavior.KeyInfo);
                    dt = mySqlDataReader.GetSchemaTable();
                    mySqlConnection.Close();
                }
                else if (_model.ConnType == ConnectionTypes.SQL_CE || _model.ConnType == ConnectionTypes.SQL_Server)
                {
                    string query = $"USE {database}; SELECT TOP 1 * FROM {tablename}; ";
                    SqlConnection sqlConnection = new SqlConnection(_model.ConnString);
                    SqlCommand SqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    SqlDataReader SqlDataAdapter = SqlCommand.ExecuteReader(CommandBehavior.KeyInfo);
                    dt = SqlDataAdapter.GetSchemaTable();
                    sqlConnection.Close();
                }
                else if (_model.ConnType == ConnectionTypes.PostgreSQL)
                {
                    string query = $"SELECT * FROM {database}.{tablename} limit 1";

                    NpgsqlConnection npgSqlConnection = new NpgsqlConnection(_model.ConnString);
                    NpgsqlCommand npgSqlCommand = new NpgsqlCommand(query, npgSqlConnection);
                    npgSqlConnection.Open();
                    NpgsqlDataReader SqlDataAdapter = npgSqlCommand.ExecuteReader(CommandBehavior.KeyInfo);
                    dt = SqlDataAdapter.GetSchemaTable();
                    npgSqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Errors.Add(ex);
            }

            dt.TableName = tablename;
            return dt;
        }
        public DataTable GetSchemaInfo(string tablename, string database)
        {
            DataTable dt = new DataTable();

            try
            {
                if (_model.ConnType == ConnectionTypes.MySQL)
                {

                    string query = $"DESCRIBE {database}.{tablename}";

                    MySqlConnection mySqlConnection = new MySqlConnection(_model.ConnString);
                    MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);
                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
                    mySqlDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    mySqlConnection.Open();
                    mySqlDataAdapter.Fill(dt);
                    mySqlConnection.Close();
                }
                else if (_model.ConnType == ConnectionTypes.SQL_CE || _model.ConnType == ConnectionTypes.SQL_Server)
                {


                    string query = $"USE {database}; select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{tablename}';";
                    SqlConnection sqlConnection = new SqlConnection(_model.ConnString);
                    SqlCommand SqlCommand = new SqlCommand(query, sqlConnection);
                    SqlDataAdapter SqlDataAdapter = new SqlDataAdapter(SqlCommand);
                    SqlDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    sqlConnection.Open();
                    SqlDataAdapter.Fill(dt);
                    sqlConnection.Close();
                }
                else if (_model.ConnType == ConnectionTypes.PostgreSQL)
                {
                    string query = $@"SELECT
										a.attname AS Field,
										t.typname || '(' || a.atttypmod || ')' AS Type,
										CASE WHEN a.attnotnull = 't' THEN 'YES' ELSE 'NO' END AS Null,
										CASE WHEN r.contype = 'p' THEN 'PRI' ELSE '' END AS Key,
										(SELECT substring(pg_catalog.pg_get_expr(d.adbin, d.adrelid), '\'(.*)\'')
											FROM
												pg_catalog.pg_attrdef d
											WHERE
												d.adrelid = a.attrelid
												AND d.adnum = a.attnum
												AND a.atthasdef) AS Default,
										'' as Extras
									FROM
										pg_class c 
										JOIN pg_attribute a ON a.attrelid = c.oid
										JOIN pg_type t ON a.atttypid = t.oid
										LEFT JOIN pg_catalog.pg_constraint r ON c.oid = r.conrelid 
											AND r.conname = a.attname
									WHERE
										c.relname = '{tablename}'
										AND a.attnum > 0
	
									ORDER BY a.attnum ";

                    NpgsqlConnection npgSqlConnection = new NpgsqlConnection(_model.ConnString);
                    NpgsqlCommand npgSqlCommand = new NpgsqlCommand(query, npgSqlConnection);
                    NpgsqlDataAdapter npgSqlDataAdapter = new NpgsqlDataAdapter(npgSqlCommand);
                    npgSqlDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    npgSqlConnection.Open();
                    npgSqlDataAdapter.Fill(dt);
                    npgSqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Errors.Add(ex);
            }

            return dt;
        }
        private DataTable ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();

            try
            {
                if (_model.ConnType == ConnectionTypes.MySQL)
                {
                    MySqlConnection mySqlConnection = new MySqlConnection(_model.ConnString);
                    MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);
                    mySqlConnection.Open();
                    MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader(CommandBehavior.KeyInfo);
                    dt = mySqlDataReader.GetSchemaTable();
                    mySqlConnection.Close();
                }
                else if (_model.ConnType == ConnectionTypes.SQL_CE || _model.ConnType == ConnectionTypes.SQL_Server)
                {
                    SqlConnection sqlConnection = new SqlConnection(_model.ConnString);
                    SqlCommand SqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    SqlDataReader SqlDataAdapter = SqlCommand.ExecuteReader(CommandBehavior.KeyInfo);
                    dt = SqlDataAdapter.GetSchemaTable();
                    sqlConnection.Close();
                }
                else if (_model.ConnType == ConnectionTypes.PostgreSQL)
                {
                    NpgsqlConnection npgSqlConnection = new NpgsqlConnection(_model.ConnString);
                    NpgsqlCommand npgSqlCommand = new NpgsqlCommand(query, npgSqlConnection);
                    npgSqlConnection.Open();
                    NpgsqlDataReader SqlDataAdapter = npgSqlCommand.ExecuteReader(CommandBehavior.KeyInfo);
                    dt = SqlDataAdapter.GetSchemaTable();
                    npgSqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Errors.Add(ex);
            }

            dt.TableName = "QueryToEntity";
            return dt;
        }



        public List<string> DataBases()
        {
            List<string> result = new List<string>();
            try
            {
                switch (_model.ConnType)
                {
                    case ConnectionTypes.MySQL:
                        result = connection.Query<string>("SHOW DATABASES").ToList();
                        break;
                    case ConnectionTypes.SQL_CE:
                        result = connection.Query<string>("SELECT name FROM master.sys.databases").ToList();
                        break;
                    case ConnectionTypes.PostgreSQL:
                        result = connection.Query<string>("SELECT datname FROM pg_database; ").ToList();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Errors.Add(ex);
            }


            return result;
        }
        public List<string> Tables(string database)
        {
            if (string.IsNullOrEmpty(database))
            {
                return new List<string>();
            }
            List<string> result = new List<string>();
            try
            {
                switch (_model.ConnType)
                {
                    case ConnectionTypes.MySQL:
                        result = connection.Query<string>($"SHOW TABLES FROM {database};").ToList();
                        break;
                    case ConnectionTypes.SQL_CE:
                        result = connection.Query<string>($"SELECT Distinct TABLE_NAME FROM {database}.information_schema.TABLES;").ToList();
                        break;
                    case ConnectionTypes.PostgreSQL:
                        result = connection.Query<string>($"SELECT * FROM pg_catalog.pg_tables WHERE tableowner = '{database}'; ").ToList();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Errors.Add(ex);
            }



            return result;
        }

    }
}
