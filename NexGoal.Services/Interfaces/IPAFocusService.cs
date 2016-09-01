namespace NexGoal.Services.Interfaces
{
    using Fanex.Data.Repository;
    using NexGoal.Core.Model;
    using NexGoal.Criteria.PAFocus;
    using NexGoal.Model;
    using NexGoal.Model.PAFocus;
    using System.Collections.Generic;

    public interface IPAFocusService
    {
        IEnumerable<FocusProcessSettingModel> GetPAFocusProcessSettingByPeriodId(int periodId);

        CommandResult UpdateProcessSetting(int periodId, IEnumerable<FocusProcessSettingModel> process);

        CommandResult UpdatePAFocus(int modifiedBy, IEnumerable<PAFocusModel> evaluations);

        IEnumerable<PAFocusModel> GetPAFocus(ICriteria criteria);

        IEnumerable<PAFocusModel> GetPAFocusByEvaluator(GetPAFocusByEvaluatorCriteria criteria);

        IEnumerable<PAFocusModel> GetPAFocusByReviewer(GetPAFocusByReviewerCriteria criteria);

        IEnumerable<PAFocusModel> GetPAFocusByManagement(GetPAFocusByManagementCriteria criteria);
    }
}