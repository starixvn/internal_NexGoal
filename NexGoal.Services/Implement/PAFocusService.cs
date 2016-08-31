namespace NexGoal.Services.Interfaces
{
    using NexGoal.Core.Model;
using NexGoal.Core.Services;
using NexGoal.Criteria.PAFocus;
using NexGoal.Model;
using System.Collections.Generic;

    public class PAFocusService : BaseService, IPAFocusService
    {
        public IEnumerable<PAFocusProcessSetting> GetPAFocusProcessSettingByPeriodId(int periodId)
        {
            IEnumerable<PAFocusProcessSetting> processSettings = new List<PAFocusProcessSetting>();
            var criteria = new GetPAFocusProcessSetingByIdCriteria { PeriodId = periodId };
            if (criteria.IsValid())
            {
                processSettings = GetData<PAFocusProcessSetting>(criteria);
            }

            return processSettings;
        }

        public CommandResult UpdateProcessSetting(int periodId, IEnumerable<PAFocusProcessSetting> process)
        {
            var result = new CommandResult();
            var criteria = new UpdatePAFocusProcessSetingCriteria
            {
                Processes = process,
                PeriodId = periodId
            };
            if (criteria.IsValid())
            {
                result = Repository.Insert(criteria);
            }

            return result;
        }
    }
}