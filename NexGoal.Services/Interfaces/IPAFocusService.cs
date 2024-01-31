namespace NexGoal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using NexGoal.Models.Common;
    using NexGoal.Models.PAFocus;
    using NexGoal.Models.PerformanceRecord;

    public interface IPAFocusService
    {
        Task<IEnumerable<PeriodBaseModel>> GetPeriodAsync();

        Task<IEnumerable<Group>> GetGroupAsync(int actorId);

        Task<IEnumerable<Employee>> GetMemberAsync(int actorId, string groupIds = null);

        Task<IEnumerable<PAFocusModel>> GetPAFocusAsync(GetPAFocusParam param);

        Task<CommandResult> EditPAFocusAsync(int actorId, IEnumerable<EditPAFocusModel> paFocuses);

        Task<PerformanceRecord> GetLastPerformanceRecordByEmployeeAsync(int employeeId, DateTime fromDate, DateTime toDate);
    }
}