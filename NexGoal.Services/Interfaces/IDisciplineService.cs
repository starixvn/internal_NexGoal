namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Discipline;

    public interface IDisciplineService
    {
        Task<IEnumerable<DisciplineModel>> GetDisciplineAsync(GetDisciplineParam param);

        Task<CommandResult> EditStatusAsync(int disciplineId, bool isActive, int modifiedBy);

        Task<CommandResult> CreateAsync(AddDisciplineModel model);

        Task<CommandResult> EditAsync(EditDisciplineModel model);
    }
}