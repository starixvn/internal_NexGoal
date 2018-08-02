namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.FlowTemplate;

    public interface IFlowTemplateService
    {
        Task<IEnumerable<FlowTemplateModel>> GetAllAsync(int? targetTypeId = null, bool? isActive = null);

        Task<FlowTemplateModel> GetByIdAsync(int flowTempalteId);

        Task<CommandResult> CreateAsync(CreateFlowTemplateModel flowTemplate);

        Task<CommandResult> EditAsync(EditFlowTemplateModel flowTemplate);

        Task<CommandResult> EditStatusAsync(int flowTemplateId, bool isActive);

        Task<CommandResult> DeleteAsync(int flowTemplateId);
    }
}