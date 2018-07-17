namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.EOQY;

    public interface IEOQService
    {
        Task<IEnumerable<EOQYModel>> GetAllEOQAsync(int? year = null, int? statusId = null);

        Task<EOQYModel> GetEOQByIdAsync(int eoqId);

        Task<IEnumerable<int>> GetYearsAsync();

        Task<CommandResult> CreateEOQAsync(CreateEOQModel eoq);

        Task<CommandResult> EditEOQAsync(EOQYModel eoq);

        Task<CommandResult> DeleteEOQAsync(int eoqId);
    }
}