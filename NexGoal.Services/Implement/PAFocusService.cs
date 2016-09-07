namespace NexGoal.Services.Interfaces
{
    using Fanex.Data.Repository;
    using NexGoal.Core.Model;
    using NexGoal.Core.Services;
    using NexGoal.Criteria.PAFocus;
    using NexGoal.Model;
    using NexGoal.Model.PAFocus;
    using NexGoal.Repositories.Implement;
    using NexGoal.Repositories.Interfaces;
    using System.Collections.Generic;

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

        public CommandResult UpdateProcessSetting(UpdatePAFocusProcessSetingCriteria criteria)
        {
            var result = new CommandResult();
            if (criteria.IsValid())
            {
                result = Repository.Insert(criteria);
            }

            return result;
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
                result = Repository.Update(criteria);
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
                result = Repository.Update(criteria);
            }

            return result;
        }
    }
}