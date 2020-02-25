using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.Architecture
{
    public class PeasyConstants
    {
        public const string ReplaceNameSpace = "[namespace]";
        public const string ReplaceProjectName = "[projectName]";
        public string PeasyDomainBaseDefaultTemplate = $@"using Peasy;
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
        public string PeasyDataProxyDefaultTemplate = $@"using Peasy;
namespace {ReplaceNameSpace}
{{
{{
    public interface {ReplaceProjectName}DataProxy<T> : IServiceDataProxy<T, long>
    {{
    }}
}}";
        public string PeasyServiceDefaultTemplate = $@"using Peasy;
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
    }
}
