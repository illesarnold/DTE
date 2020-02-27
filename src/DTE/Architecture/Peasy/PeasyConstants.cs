using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.Architecture
{
    public class PeasyConstants
    {
        private const string ReplaceNameSpace = "[namespace]";
        private const string ReplaceProjectName = "[projectName]";
        private const string ReplaceModelCode = "[ModelCode]";
        private const string ReplaceModelName = "[ModelName]";
        private static string PeasyDomainBaseDefaultTemplate = $@"using Peasy;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace {ReplaceNameSpace}
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
        private static string PeasyDataProxyDefaultTemplate = $@"using Peasy;
namespace {ReplaceNameSpace}
{{
{{
    public interface {ReplaceProjectName}DataProxy<T> : IServiceDataProxy<T, long>
    {{
    }}
}}";
        private static string PeasyServiceDefaultTemplate = $@"using Peasy;
using {ReplaceNameSpace}.DataProxy;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace {ReplaceNameSpace}
{{
    public abstract class {ReplaceProjectName}ServiceBase<T> : BusinessServiceBase<T, long> where T : IDomainObject<long>, new()
    {{
        public OrdersDotComServiceBase(IOrdersDotComDataProxy<T> dataProxy) : base(dataProxy)
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
        private static string PeasyDomainTemplate = $@"using System.ComponentModel.DataAnnotations;

namespace {ReplaceNameSpace}.Domain
{{
    {ReplaceModelCode}
}}";
        private static string PeasyDataProxyTemplate = $@"using Orders.com.BLL.Domain;

namespace {ReplaceNameSpace}.DataProxy
{{
    public interface I{ReplaceModelName}DataProxy : I{ReplaceProjectName}DataProxy<{ReplaceModelName}>
    {{
    }}
}}";
        private static string PeasyServiceTemplate = $@"using {ReplaceNameSpace}.Domain;
using Peasy;

namespace {ReplaceNameSpace}.Services
{{
    public interface I{ReplaceModelName}Service : IService<{ReplaceModelName}, long>
    {{
    }}
}}";


        public static string GetDomainTemplate(string nameSpace,string modelCode)
        {
            return PeasyDomainTemplate.Replace(ReplaceNameSpace, nameSpace).Replace(ReplaceModelCode, modelCode);
        }
        public static string GetDataProxyTemplate(string nameSpace,string projectName, string modelName)
        {
            return PeasyDataProxyTemplate
                .Replace(ReplaceNameSpace, nameSpace)
                .Replace(ReplaceProjectName, projectName)
                .Replace(ReplaceModelName, modelName);
        }
        public static string GetServiceTemplate(string nameSpace, string modelName)
        {
            return PeasyServiceTemplate.Replace(ReplaceNameSpace, nameSpace).Replace(ReplaceModelName, modelName);
        }

    }
}

