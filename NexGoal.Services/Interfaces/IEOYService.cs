namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.EOQY;

    public interface IEOYService
    {
        Task<IEnumerable<EOQYModel>> GetAllEOYAsync(int? statusId = null);

        Task<EOQYModel> GetEOYByIdAsync(int eoyId);

        Task<CommandResult> CreateEOYAsync(CreateEOYModel eoy);

        Task<CommandResult> EditEOYAsync(EOQYModel eoy);

        Task<CommandResult> DeleteEOYAsync(int eoyId);
    }
}