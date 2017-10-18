namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PAFocus;
    using NexGoal.Model;
    using NexGoal.Model.PAFocus;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;

    public class PAFocusService : BaseService, IPAFocusService
    {
        private readonly IPAFocusRepository focusRepository;

        public PAFocusService()
            : this(null)
        {
        }

        public PAFocusService(IPAFocusRepository focusRepository)
        {
            this.focusRepository = focusRepository ?? new PAFocusRepository();
        }

        public IEnumerable<int> GetProcessId(int periodId, int evaluatorId)
        {
            IEnumerable<int> processSettings = new List<int>();
            var criteria = new GetPAFocusProcessIdCriteria { PeriodId = periodId, EvaluatorId = evaluatorId };
            if (criteria.IsValid())
            {
                processSettings = GetData<int>(criteria);
            }

            return processSettings;
        }

        public IEnumerable<FocusProcessSettingModel> GetPAFocusProcessSettingByPeriodId(int periodId)
        {
            IEnumerable<FocusProcessSettingModel> processSettings = new List<FocusProcessSettingModel>();
            var criteria = new GetPAFocusProcessSetingByIdCriteria { PeriodId = periodId };
            if (criteria.IsValid())
            {
                processSettings = GetData<FocusProcessSettingModel>(criteria);
            }

            return processSettings;
        }

        public CommandResult UpdatePAFocus(int modifiedBy, IEnumerable<PAFocusModel> evaluations)
        {
            var result = new CommandResult();
            var criteria = new UpdatePAFocusCriteria
            {
                ModifiedBy = modifiedBy,
                Evaluations = evaluations
            };
            if (criteria.IsValid())
            {
                result = Repository.ExecuteNonQuery(criteria);
            }

            return result;
        }

        public IEnumerable<PAFocusProcessModel> GetPAFocusProcess(int periodId)
        {
            var criteria = new GetPAFocusProcessByPeriodIdCriteria { PeriodId = periodId };
            IEnumerable<PAFocusProcessModel> result = new List<PAFocusProcessModel>();
            if (criteria.IsValid())
            {
                result = GetData<PAFocusProcessModel>(criteria);
            }

            return result;
        }

        public IEnumerable<PAFocusModel> GetPAFocus(ICriteria criteria)
        {
            var list = this.focusRepository.GetPAFocus(criteria);
            foreach (PAFocusModel focus in list)
            {
                focus.Decrypt();
            }

            return list;
        }

        public CommandResult GeneratePAFocus(int periodId)
        {
            var result = new CommandResult();
            var criteria = new GeneratePAFocusCriteria
            {
                PeriodId = periodId
            };
            if (criteria.IsValid())
            {
                result = Repository.ExecuteNonQuery(criteria);
            }

            return result;
        }
    }
}