namespace NexGoal.Tools
{
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

        public EPAViewModel GetEPAInfoToChangeCertificate(IDbSetting setting, GetEPAInfoToChangeCertificateCriteria criteria)
        {
            EPAViewModel epaInfo = new EPAViewModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = commonRepository.GetEPAInfoToChangeCertificate(setting, criteria);
            }

            return epaInfo;
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
    }
}