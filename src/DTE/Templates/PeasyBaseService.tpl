using Peasy;
using [BusinessNameSpace].DataProxy;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace [BusinessNameSpace]
{
    public abstract class [SolutionName]ERPServiceBase<T> : BusinessServiceBase<T, long> where T : IDomainObject<long>, new()
    {
        public [SolutionName]ServiceBase(I[SolutionName]DataProxy<T> dataProxy) : base(dataProxy)
        {
        }

        protected override IEnumerable<ValidationResult> GetAllErrorsForInsert(T entity, ExecutionContext<T> context)
        {
            var validationErrors = GetValidationResultsForInsert(entity, context);
            if (!validationErrors.Any())
            {
                var businessRuleErrors = GetBusinessRulesForInsert(entity, context).GetValidationResults();
                validationErrors = validationErrors.Concat(businessRuleErrors);
            }
            return validationErrors;
        }

        protected override async Task<IEnumerable<ValidationResult>> GetAllErrorsForInsertAsync(T entity, ExecutionContext<T> context)
        {
            var validationErrors = GetValidationResultsForInsert(entity, context);
            if (!validationErrors.Any())
            {
                var businessRuleErrors = await GetBusinessRulesForInsertAsync(entity, context);
                validationErrors = validationErrors.Concat(await businessRuleErrors.GetValidationResultsAsync());
            }
            return validationErrors;
        }

        protected override IEnumerable<ValidationResult> GetAllErrorsForUpdate(T entity, ExecutionContext<T> context)
        {
            var validationErrors = GetValidationResultsForUpdate(entity, context);
            if (!validationErrors.Any())
            {
                var businessRuleErrors = GetBusinessRulesForUpdate(entity, context).GetValidationResults();
                validationErrors = validationErrors.Concat(businessRuleErrors);
            }
            return validationErrors;
        }

        protected override async Task<IEnumerable<ValidationResult>> GetAllErrorsForUpdateAsync(T entity, ExecutionContext<T> context)
        {
            var validationErrors = GetValidationResultsForUpdate(entity, context);
            if (!validationErrors.Any())
            {
                var businessRuleErrors = await GetBusinessRulesForUpdateAsync(entity, context);
                validationErrors = validationErrors.Concat(await businessRuleErrors.GetValidationResultsAsync());
            }
            return validationErrors;
        }
    }
}