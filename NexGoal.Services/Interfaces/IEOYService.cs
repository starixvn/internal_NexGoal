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

        Task<CommandResult> CreateAsync(CreateEOYModel eoy);

        Task<CommandResult> EditAsync(EOQYModel eoy);

        Task<CommandResult> DeleteAsync(int eoyId);
    }
}