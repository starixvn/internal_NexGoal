namespace NexGoal.Tools
{
    using Fanex.Data;
    using System.Collections.Generic;

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

        public IEnumerable<EPAModel> GetEPAInfo(string connectionString, int idFrom, int idTo)
        {
            IEnumerable<EPAModel> epaInfos = new List<EPAModel>();
            var criteria = new GetEPAInfoToChangeCertificateCriteria { EPAIdFrom = idFrom, EPAIdTo = idTo };
            var setting = new DbSetting { ConnectionString = connectionString, SpName = criteria.GetSettingKey() };
            if (criteria != null && criteria.IsValid())
            {
                epaInfos = commonRepository.GetEPAInfoToChangeCertificate(setting, criteria);
            }

            return epaInfos;
        }

        public CommandResult UpdateEPAScore(string connectionString, IEnumerable<EPAModel> epas)
        {
            var commandResult = new CommandResult();
            var criteria = new UpdateEPAScoreCriteria { EpaInfos = epas };
            criteria.ConvertToXml();
            var setting = new DbSetting { ConnectionString = connectionString, SpName = criteria.GetSettingKey() };
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

        public IEnumerable<PAFocusModel> GetAllPAFocus(string connectionString, int idFrom, int idTo)
        {
            var criteria = new GetAllPAFocusCriteria { PAFocusIdFrom = idFrom, PAFocusIdTo = idTo };
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