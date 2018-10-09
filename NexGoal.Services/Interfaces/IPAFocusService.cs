namespace NexGoal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NexGoal.Models.Common;
    using NexGoal.Models.PAFocus;

    public interface IPAFocusService
    {
        Task<IEnumerable<PeriodBaseModel>> GetPeriodAsync();

        Task<IEnumerable<GroupBaseModel>> GetGroupAsync(int actorId);

        Task<IEnumerable<EmployeeBaseModel>> GetMemberAsync(int actorId);

        Task<IEnumerable<PAFocusModel>> GetPAFocusAsync(GetPAFocusParam param);
    }
}