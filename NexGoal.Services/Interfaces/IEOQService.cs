namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.EOQY;

    public interface IEOQService
    {
        Task<IEnumerable<EOQYModel>> GetAllAsync(int? year = null, int? statusId = null);

        Task<EOQYModel> GetByIdAsync(int eoqId);

        Task<IEnumerable<int>> GetYearsAsync();

        Task<CommandResult> CreateAsync(int actorId, CreateEOQModel eoq);

        Task<CommandResult> EditAsync(int actorId, EditEOQModel eoq);
    }
}