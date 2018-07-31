namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Period;

    public interface IPeriodService
    {
        Task<IEnumerable<PeriodModel>> GetPeriods(int? targetTypeId, bool? isActive);

        Task<IEnumerable<TargetTypeModel>> GetTargetTypes();

        Task<CommandResult> Create(CreatePeriodModel model);

        Task<CommandResult> EditStatus(int periodId, bool isActive, int modifiedBy);
    }
}