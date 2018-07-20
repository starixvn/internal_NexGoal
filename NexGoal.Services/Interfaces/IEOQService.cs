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

        Task<CommandResult> CreateAsync(CreateEOQModel eoq);

        Task<CommandResult> EditAsync(EOQYModel eoq);

        Task<CommandResult> DeleteAsync(int eoqId);

        Task<IEnumerable<NominatorSettingModel>> GetNominatorSettingAsync(
            int eoqId,
            string keyword = null,
            string groupIds = null,
            int? roleId = null,
            bool isShowAll = false);

        Task<CommandResult> EditNominatorSettingAsync(EditNominatorSettingModel setting);
    }
}