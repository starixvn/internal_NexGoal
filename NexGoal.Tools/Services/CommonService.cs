namespace NexGoal.Tools
{
    using System.Collections.Generic;
    using Fanex.Data;

    public class CommonService : ICommonService
    {
        private readonly ICommonRepository commonRepository;
        private readonly IBaseRepository repository;

        public CommonService()
            : this(null, null)
        {
        }

        public CommonService(
            IBaseRepository repository,
            ICommonRepository commonRepository)
        {
            this.repository = repository ?? new BaseRepository();
            this.commonRepository = commonRepository ?? new CommonRepository();
        }

        public CommandResult ResetScore(IDbSetting setting, ResetScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = repository.Update(setting, criteria);
            }

            return commandResult;
        }

        public IList<EPAModel> GetEPAInfoToChangeCertificate(IDbSetting setting, GetEPAInfoToChangeCertificateCriteria criteria)
        {
            IList<EPAModel> epaInfos = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                epaInfos = commonRepository.GetEPAInfoToChangeCertificate(setting, criteria);
            }

            return epaInfos;
        }

        public CommandResult UpdateEPAScore(IDbSetting setting, UpdateEPAScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = repository.Update(setting, criteria);
            }

            return commandResult;
        }

        public CommandResult UpdatePAFocus(string connectionString, IEnumerable<PAFocusModel> focus)
        {
            CommandResult commandResult = new CommandResult();
            var criteria = new UpdatePAFocusCriteria { Evaluations = focus };
            var setting = new DbSetting { ConnectionString = connectionString, SpName = criteria.GetSettingKey() };
            if (criteria != null && criteria.IsValid())
            {
                commandResult = repository.Update(setting, criteria);
            }

            return commandResult;
        }

        public IEnumerable<PAFocusModel> GetAllPAFocus(string connectionString)
        {
            var criteria = new GetAllPAFocusCriteria();
            var setting = new DbSetting { ConnectionString = connectionString, SpName = criteria.GetSettingKey() };
            IEnumerable<PAFocusModel> focuses = new List<PAFocusModel>();

            if (criteria != null && criteria.IsValid())
            {
                focuses = commonRepository.GetAllPAFocus(setting, criteria);
            }

            return focuses;

        }
    }
}