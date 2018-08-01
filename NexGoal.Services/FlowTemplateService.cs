namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.FlowTemplate;
    using NexGoal.Models.FlowTemplate;

    public class FlowTemplateService : IFlowTemplateService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;

        public FlowTemplateService(
            IBaseService baseService,
            ISessionService sessionService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
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

        public async Task<CommandResult> CreateAsync(CreateFlowTemplateModel flowTemplate)
        {
            var criteria = new CreateFlowTemplateCriteria
            {
                Name = flowTemplate.Name,
                TargetTypeId = flowTemplate.TargetTypeId,
                Description = flowTemplate.Description,
                IsActive = flowTemplate.IsActive,
                CreatedBy = sessionService.EmployeeId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}