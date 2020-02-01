using DTE.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.CORE
{
    public class DTECore : IDTE
    {
        public IDTE DTEService { get; set; }
        private ISettings _settings;
        public DTECore(SupportedConnectionsTypes type, string connection_String, ISettings settings)
        {
            _settings = settings;
            DTEService = DteCoreFactory.CreateDTECore(type, connection_String);
        }

        private string CreateModel(DataTable dt, DataTable table_describe = null)
        {
            var tablename = dt.TableName.Split('.').Last();
            var class_annotations = "";
            string properties = "";
            var class_name = Helpers.ModelCreateHelper.ColumnNameToPropName(tablename).Replace("_", "");


            if (_settings.DataAnnotations)
                class_annotations += $"[{_settings.Attributes.Table}(\"{tablename}\")]"+Environment.NewLine;
            if (_settings.DataMember)
                class_annotations += string.IsNullOrEmpty(class_annotations) ?  $"{Environment.NewLine}[DataContract]" : "[DataContract]";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var auto_increment = bool.Parse(dt.Rows[i]["IsAutoIncrement"]?.ToString());
                var column_name = dt.Rows[i]["ColumnName"].ToString();
                var allow_null = bool.Parse(dt.Rows[i]["AllowDBNull"].ToString());
                var column_type = dt.Rows[i]["DataType"].ToString();
                var isKey = bool.Parse(dt.Rows[i]["IsKey"].ToString());

                var name = _settings.CaseSensitivity ? column_name : Helpers.ModelCreateHelper.ColumnNameToPropName(column_name.ToString());
                var cType = GetCsharpType(column_type, allow_null);
                var comment = "";
                var annotations = "";

                if (_settings.Comments)
                {
                    for (int j = 0; j < table_describe?.Columns.Count; j++)
                    {
                        comment += $"  //  {table_describe.Columns[j].ColumnName}: {table_describe?.Rows[i][j].ToString()} ";
                    }
                }
                if (isKey)
                {
                    if (_settings.DataAnnotations && auto_increment)
                        annotations += $"[{_settings.Attributes.Key}]"+Environment.NewLine;
                    if (_settings.DataAnnotations && auto_increment == false)
                        annotations += $"[{_settings.Attributes.ExplicitKey}]" + Environment.NewLine;
                }
                if (_settings.DataMember)
                    annotations += $"[DataMember]";
                if (_settings.FullProp)
                    properties = FullPropText(properties, column_name, name, comment, cType, annotations);
                else
                    properties = PropText(properties, column_name, name, comment, cType, annotations);

            }

            return _settings.ClassTemplate?.Replace("[Annotations]", class_annotations)
                                          ?.Replace("[Prefix]", _settings.Prefix)
                                          ?.Replace("[Name]", class_name)
                                          ?.Replace("[Postfix]", _settings.Postfix)
                                          ?.Replace("[Properties]", properties);
        }
      
        private string PropText(string model, string column_name, string name, string comment, string cType, string annotations)
        {
            var template = _settings.PropTemplate;
            if (string.IsNullOrEmpty(annotations))
                template = template.Trim();

            var propText = template?.Replace("[Type]", cType)
                                        ?.Replace("[PrivateName]", column_name)
                                        ?.Replace("[PublicName]", name)
                                        ?.Replace("[Comment]", comment)
                                        ?.Replace("[Annotations]", annotations);
            

            return model += propText+Environment.NewLine;
        }
        private string FullPropText(string model, string column_name, string name, string comment, string cType, string annotations)
        {
            var template = _settings.FullPropTemplate;
            if (string.IsNullOrEmpty(annotations))
                template = template.Trim();
            var propText = template?.Replace("[Type]", cType)
                                          ?.Replace("[PrivateName]", column_name)
                                          ?.Replace("[PublicName]", name)
                                          ?.Replace("[Comment]", comment)
                                          ?.Replace("[Annotations]", annotations);

            if (string.IsNullOrEmpty(annotations))
                propText.Trim();
            return model += propText + Environment.NewLine;
        }
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
        public string CreateModel(string database, string tablename)
        {
            DataTable dt = GetFirstRowWithSchemaInfo(tablename, database);
            var table_describe = DTEService.GetSchema(database,dt.TableName);

            return CreateModel(dt, table_describe);
        }
        public async Task<string> CreateModelAsync(string database, string tablename)
        {
            DataTable dt = GetFirstRowWithSchemaInfo(tablename, database);
            var table_describe = await DTEService.GetSchemaAsync(database, dt.TableName);

            return CreateModel(dt, table_describe);
        }
        public string CreateModelByQuery(string query)
        {
            DataTable dt = DTEService.ExecuteQueryWithTableInfo(query);

            return CreateModel(dt);
        }
        public DataTable GetFirstRowWithSchemaInfo(string tablename, string database)
        {
            DataTable dt = DTEService.GetFirstRowWithSchemaInfo(database, tablename);
            dt.TableName = tablename;
            return dt;
        }
        public DataTable GetSchema(string tablename, string database)
        {
            DataTable dt = DTEService.GetSchema(database, tablename);
            dt.TableName = tablename;
            return dt;
        }
        public IEnumerable<string> GetDatabases()
        {
            return DTEService.GetDatabases();
        }
        public IEnumerable<string> GetTables(string database_name)
        {
            return DTEService.GetTables(database_name);
        }
        public IEnumerable<string> GetColumns(string database_name, string table_name)
        {
            return DTEService.GetColumns(database_name,table_name);
        }
        public DataTable ExecuteQueryWithTableInfo(string query)
        {
            return DTEService.ExecuteQueryWithTableInfo(query);
        }
        public async Task<IEnumerable<string>> GetDatabasesAsync()
        {
            return await DTEService.GetDatabasesAsync();
        }
        public async Task<IEnumerable<string>> GetTablesAsync(string database_name)
        {
            return await DTEService.GetTablesAsync(database_name);
        }
        public async Task<IEnumerable<string>> GetColumnsAsync(string database_name, string table_name)
        {
            return await DTEService.GetColumnsAsync(database_name, table_name);
        }
        public async Task<DataTable> GetSchemaAsync(string database_name, string table_name)
        {
            return await DTEService.GetSchemaAsync(database_name, table_name);
        }
    }
}
