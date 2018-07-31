namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.FlowTemplate;
    using NexGoal.Models.FlowTemplate;

    public class FlowTemplateService
    {
        private readonly IBaseService baseService;

        public FlowTemplateService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<FlowTemplateModel>> GetAllAsync(int? targetTypeId = null, bool? isActive = null)
        {
            var criteria = new GetFlowTemplateCriteria
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };

            return await baseService.GetDataAsync<FlowTemplateModel>(criteria).ConfigureAwait(false);
        }

        public async Task<FlowTemplateModel> GetByIdAsync(int flowTempalteId)
        {
            var criteria = new GetFlowTemplateByIdCriteria
            {
                FlowTemplateId = flowTempalteId
            };

            return await baseService.GetDataEntityAsync<FlowTemplateModel>(criteria).ConfigureAwait(false);
        }
    }
}