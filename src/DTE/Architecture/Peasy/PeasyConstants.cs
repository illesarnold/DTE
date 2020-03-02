using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.Architecture
{
    public class PeasyConstants
    {
        private const string ReplaceBusinessNameSpace = "[business_namespace]";
        private const string ReplaceDataLayerNameSpace = "[dal_namespace]";
        private const string ReplaceProjectName = "[projectName]";
        private const string ReplaceModelCode = "[ModelCode]";
        private const string ReplaceModelName = "[ModelName]";
        private static string PeasyDomainBaseTemplate = $@"using Peasy;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace {ReplaceBusinessNameSpace}
{{
    public abstract class DomainBase : IDomainObject<long>
    {{
        
        public abstract long ID {{ get; set; }}

        public string Self {{ get; set; }}

        [Editable(false)]
        public int? CreatedBy {{ get; set; }}

        [Editable(false)]
        public DateTime CreatedDatetime {{ get; set; }}

        [Editable(false)]
        public int? LastModifiedBy {{ get; set; }}

        [Editable(false)]
        public DateTime? LastModifiedDatetime {{ get; set; }}
    }}
}}
";
        private static string PeasyDataProxyBaseTemplate = $@"using Peasy;
namespace {ReplaceBusinessNameSpace}.DataProxy
{{
{{
    public interface I{ReplaceProjectName}DataProxy<T> : IServiceDataProxy<T, long>
    {{
    }}
}}";
        private static string PeasyServiceBaseTemplate = $@"using Peasy;
using {ReplaceBusinessNameSpace}.DataProxy;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace {ReplaceBusinessNameSpace}
{{
    public abstract class {ReplaceProjectName}ServiceBase<T> : BusinessServiceBase<T, long> where T : IDomainObject<long>, new()
    {{
        public {ReplaceProjectName}ServiceBase(I{ReplaceProjectName}Proxy<T> dataProxy) : base(dataProxy)
        {{
        }}

        protected override IEnumerable<ValidationResult> GetAllErrorsForInsert(T entity, ExecutionContext<T> context)
        {{
            var validationErrors = GetValidationResultsForInsert(entity, context);
            if (!validationErrors.Any())
            {{
                var businessRuleErrors = GetBusinessRulesForInsert(entity, context).GetValidationResults();
                validationErrors = validationErrors.Concat(businessRuleErrors);
            }}
            return validationErrors;
        }}

        protected override async Task<IEnumerable<ValidationResult>> GetAllErrorsForInsertAsync(T entity, ExecutionContext<T> context)
        {{
            var validationErrors = GetValidationResultsForInsert(entity, context);
            if (!validationErrors.Any())
            {{
                var businessRuleErrors = await GetBusinessRulesForInsertAsync(entity, context);
                validationErrors = validationErrors.Concat(await businessRuleErrors.GetValidationResultsAsync());
            }}
            return validationErrors;
        }}

        protected override IEnumerable<ValidationResult> GetAllErrorsForUpdate(T entity, ExecutionContext<T> context)
        {{
            var validationErrors = GetValidationResultsForUpdate(entity, context);
            if (!validationErrors.Any())
            {{
                var businessRuleErrors = GetBusinessRulesForUpdate(entity, context).GetValidationResults();
                validationErrors = validationErrors.Concat(businessRuleErrors);
            }}
            return validationErrors;
        }}

        protected override async Task<IEnumerable<ValidationResult>> GetAllErrorsForUpdateAsync(T entity, ExecutionContext<T> context)
        {{
            var validationErrors = GetValidationResultsForUpdate(entity, context);
            if (!validationErrors.Any())
            {{
                var businessRuleErrors = await GetBusinessRulesForUpdateAsync(entity, context);
                validationErrors = validationErrors.Concat(await businessRuleErrors.GetValidationResultsAsync());
            }}
            return validationErrors;
        }}
    }}
}}
";
        private static string PeasyRepositoryBaseDefaultTemplate = $@"using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Dapper.Contrib.Extensions;
using System.Threading.Tasks;
using {ReplaceBusinessNameSpace}.Domain;
using {ReplaceBusinessNameSpace}.DataProxy;
using MySql.Data.MySqlClient;

namespace {ReplaceDataLayerNameSpace}.Repository
{{
    abstract class RepositoryBase<T> : I{ReplaceProjectName}DataProxy<T> where T : DomainBase, new()
    {{
        protected IDbConnection _dbConnection = null;
        public BaseRepository(IDbConnection dbConnection)
        {{
            _dbConnection = dbConnection;
        }}

        protected string _baseTableName = GetTableName(typeof(T));
        public bool SupportsTransactions => true;

        public bool IsLatencyProne => false;

        public void Delete(long id)
        {{
            CreateConnection().Delete(new T() {{ ID = id }});
        }}

        public async Task DeleteAsync(long id)
        {{
            await CreateConnection().DeleteAsync(new T() {{ ID = id }});
        }}

        public IEnumerable<T> GetAll()
        {{
            return CreateConnection().GetAll<T>();
        }}

        public async Task<IEnumerable<T>> GetAllAsync()
        {{
            return await CreateConnection().GetAllAsync<T>();
        }}

        public T GetByID(long id)
        {{
            return CreateConnection().Get<T>(id);
        }}

        public async Task<T> GetByIDAsync(long id)
        {{
            return await CreateConnection().GetAsync<T>(id);
        }}

        public T Insert(T entity)
        {{
            var id = CreateConnection().Insert(entity);
            return CreateConnection().Get<T>(id);
        }}

        public async Task<T> InsertAsync(T entity)
        {{
            var id = await CreateConnection().InsertAsync(entity);
            return await CreateConnection().GetAsync<T>(id);
        }}

        public T Update(T entity)
        {{
            CreateConnection().Update(entity);
            return CreateConnection().Get<T>(entity.ID);
        }}

        public async Task<T> UpdateAsync(T entity)
        {{
            await CreateConnection().UpdateAsync(entity);
            return await CreateConnection().GetAsync<T>(entity.ID);
        }}

        public IDbConnection CreateConnection()
        {{
            if (_dbConnection is MySqlConnection)
                return new MySqlConnection(_dbConnection.ConnectionString);
            if (_dbConnection is MySqlConnection)
                return new SqlConnection(_dbConnection.ConnectionString);

            return null;
        }}
        private static string GetTableName(Type obj)
        {{
            var tAttribute = (TableAttribute)obj.GetCustomAttributes(typeof(TableAttribute), true)[0];

            return tAttribute.Name;
        }}
    }}
}}

";
        private static string PeasyDomainTemplate = $@"using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace {ReplaceBusinessNameSpace}.Domain
{{
    {ReplaceModelCode}
}}";
        private static string PeasyDataProxyTemplate = $@"using {ReplaceBusinessNameSpace}.Domain;

namespace {ReplaceBusinessNameSpace}.DataProxy
{{
    public interface I{ReplaceModelName}DataProxy : I{ReplaceProjectName}DataProxy<{ReplaceModelName}>
    {{
    }}
}}";
        private static string PeasyServiceTemplate = $@"using {ReplaceBusinessNameSpace}.Domain;
using Peasy;

namespace {ReplaceBusinessNameSpace}.Services
{{
    public interface I{ReplaceModelName}Service : IService<{ReplaceModelName}, long>
    {{
    }}
}}";
        private static string PeasyRepositoryTemplate = $@"using  {ReplaceBusinessNameSpace}.Domain;
using {ReplaceBusinessNameSpace}.DataProxy;

namespace {ReplaceDataLayerNameSpace}.Repository
{{
    public class {ReplaceModelName}Repository : RepositoryBase<{ReplaceModelName}>, I{ReplaceModelName}DataProxy
    {{
    }}
}}";

        public static string GetDomainTemplate(string nameSpace, string modelCode)
        {
            return PeasyDomainTemplate.Replace(ReplaceBusinessNameSpace, nameSpace).Replace(ReplaceModelCode, modelCode);
        }
        public static string GetDataProxyTemplate(string nameSpace, string projectName, string modelName)
        {
            return PeasyDataProxyTemplate
                .Replace(ReplaceBusinessNameSpace, nameSpace)
                .Replace(ReplaceProjectName, projectName)
                .Replace(ReplaceModelName, modelName);
        }
        public static string GetServiceTemplate(string nameSpace, string modelName)
        {
            return PeasyServiceTemplate.Replace(ReplaceBusinessNameSpace, nameSpace).Replace(ReplaceModelName, modelName);
        }
        public static string GetRepositoryTemplate(string dataLayerNameSpace, string businessNameSpace, string modelName)
        {
            return PeasyRepositoryTemplate
               .Replace(ReplaceDataLayerNameSpace, dataLayerNameSpace)
               .Replace(ReplaceBusinessNameSpace, businessNameSpace)
               .Replace(ReplaceModelName, modelName);
        }
        public static string GetDomainBaseTemplate(string businessNameSpace)
        {
            return PeasyDomainBaseTemplate
               .Replace(ReplaceBusinessNameSpace, businessNameSpace);
        }
        public static string GetDataProxyBaseTemplate(string businessNameSpace, string projectName)
        {
            return PeasyDataProxyBaseTemplate
               .Replace(ReplaceBusinessNameSpace, businessNameSpace)
               .Replace(ReplaceProjectName, projectName);
        }
        public static string GetServiceBaseTemplate(string businessNameSpace, string projectName)
        {
            return PeasyServiceBaseTemplate
               .Replace(ReplaceBusinessNameSpace, businessNameSpace)
               .Replace(ReplaceProjectName, projectName);
        }
        public static string GetRepositoryBaseTemplate(string dataLayerNameSpace, string businessNameSpace, string projectName)
        {
            return PeasyRepositoryBaseDefaultTemplate
               .Replace(ReplaceDataLayerNameSpace, dataLayerNameSpace)
               .Replace(ReplaceBusinessNameSpace, businessNameSpace)
               .Replace(ReplaceProjectName, projectName);
        }
    }
}

