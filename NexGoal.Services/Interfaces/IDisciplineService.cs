namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Discipline;

    public interface IDisciplineService
    {
        Task<IEnumerable<DisciplineModel>> GetDisciplines(DateTime from, DateTime to, bool? isActive, string groupIds, int? employeeId);

        Task<CommandResult> EditStatus(int disciplineId, bool isActive, int modifiedBy);

        Task<CommandResult> Create(AddDisciplineModel model);

        Task<CommandResult> Edit(EditDisciplineModel model);
    }
}