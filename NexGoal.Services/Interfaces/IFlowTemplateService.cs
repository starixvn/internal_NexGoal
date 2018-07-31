namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.FlowTemplate;

    public interface IFlowTemplateService
    {
        Task<IEnumerable<FlowTemplateModel>> GetAllAsync(int? targetTypeId = null, bool? isActive = null);

        Task<FlowTemplateModel> GetByIdAsync(int flowTempalteId);
    }
}