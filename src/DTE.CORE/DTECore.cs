using DTE.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DTE.CORE
{
    public class DTECore : IDTE
    {
        public IDTE DTEService { get; set; }

        private const string annotationTag = "[Annotations]";
        public readonly ISettings Settings;
        public DTECore(SupportedConnectionsTypes type, string connection_String, ISettings settings)
        {
            Settings = settings;
            DTEService = DteCoreFactory.CreateDTECore(type, connection_String);
        }

        readonly string indent = "\t";

        private string CreateModel(DataTable dt, DataTable table_describe = null)
        {
            var tablename = dt.TableName.Split('.').Last();
            var class_name = Helpers.ModelCreateHelper.ColumnNameToPropName(tablename).Replace("_", "");
            var class_annotations = "";
            string properties = "";

            if (Settings.DataAnnotations)
                class_annotations += $"[{Settings.Attributes.Table}(\"{tablename}\")]";
            if (Settings.DataMember)
                class_annotations += string.IsNullOrEmpty(class_annotations) ?  $"[DataContract]" : $"{Environment.NewLine}[DataContract]";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var auto_increment = bool.Parse(dt.Rows[i]["IsAutoIncrement"]?.ToString());
                var column_name = dt.Rows[i]["ColumnName"].ToString();
                var allow_null = bool.Parse(dt.Rows[i]["AllowDBNull"].ToString());
                var column_type = dt.Rows[i]["DataType"].ToString();
                var isKey = bool.Parse(dt.Rows[i]["IsKey"].ToString());

                var name = Settings.CaseSensitivity ? column_name : Helpers.ModelCreateHelper.ColumnNameToPropName(column_name.ToString());
                var cType = GetCsharpType(column_type, allow_null);
                var comment = "";
                var annotations = "";

                if (Settings.Comments)
                {
                    for (int j = 0; j < table_describe?.Columns.Count; j++)
                    {
                        comment += $"// {table_describe.Columns[j].ColumnName}: {table_describe?.Rows[i][j].ToString()} ";
                    }
                }
                if (isKey)
                {
                    if (Settings.DataAnnotations && auto_increment)
                        annotations += $"[{Settings.Attributes.Key}]";
                    if (Settings.DataAnnotations && auto_increment == false)
                        annotations += $"[{Settings.Attributes.ExplicitKey}]";
                }
                if (Settings.DataMember)
                    annotations += string.IsNullOrEmpty(annotations) ? $"[DataMember]" : Environment.NewLine+"[DataMember]";
                if (Settings.FullProp)
                    properties = FullPropText(properties, column_name, name, comment, cType, annotations);
                else
                    properties = PropText(properties, column_name, name, comment, cType, annotations);

            }

            var classString = Settings.ClassTemplate?.Replace("[Annotations]", class_annotations)
                                          ?.Replace("[Prefix]", Settings.Prefix)
                                          ?.Replace("[Name]", class_name)
                                          ?.Replace("[Postfix]", Settings.Postfix)
                                          ?.Replace("[Properties]", properties);

            return classString;
        }
      
        private string PropText(string properties, string column_name, string name, string comment, string cType, string annotations)
        {
            var template = Settings.PropTemplate;
            return ReplaceTemplateTags(ref properties, column_name, name, comment, cType, annotations, ref template);
        }

        private string ReplaceTemplateTags(ref string properties, string column_name, string name, string comment, string cType, string annotations, ref string template)
        {
            if (string.IsNullOrEmpty(annotations))
                template = template.Trim();

            var propText = template?.Replace("[Type]", cType)
                                        ?.Replace("[PrivateName]", column_name)
                                        ?.Replace("[PublicName]", name)
                                        ?.Replace("[Comment]", comment)
                                        ?.Replace(annotationTag, string.IsNullOrEmpty(annotations) ? annotationTag : annotations);

            if (!string.IsNullOrEmpty(properties))
                propText = Environment.NewLine + propText;

            var identedText = indent + propText.Replace("\n", "\n" + indent);
            var finalText = string.Join(Environment.NewLine, identedText.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(x => x.Contains(annotationTag) == false));
            return properties += finalText;
        }

        private string FullPropText(string properties, string column_name, string name, string comment, string cType, string annotations)
        {
            var template = Settings.FullPropTemplate;
            return ReplaceTemplateTags(ref properties, column_name, name, comment, cType, annotations, ref template);
        }
        private string GetCsharpType(string type, bool nullable)
        {
            string backType = "";

            backType = Settings.Types.FirstOrDefault(x => x.CType == type)?.PType ?? "";
            if (nullable && Settings.Nullable == true)
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
