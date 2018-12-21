namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.EOQY;

    public interface IEOYService
    {
        Task<IEnumerable<EOQYModel>> GetAllAsync(int? statusId = null);

        Task<EOQYModel> GetByIdAsync(int eoyId);

        Task<CommandResult> CreateAsync(int actorId, CreateEOYModel eoy);

        Task<CommandResult> EditAsync(int actorId, EditEOYModel eoy);
    }
}