namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria;
    using Fanex.Data.Repository;
    using Model;
    using Model.Nomination;

    public interface INominationService
    {
        NominatorSettingModel GetLatestEOQByNominatorId(int nominatorId);

        NominatorSettingModel GetLatestEOYByNominatorId(int nominatorId);

        IEnumerable<NominationViewModel> GetNominationByNominatorId(GetNominationByNominatorIdCriteria criteria);

        IEnumerable<NominationEPAModel> GetNominationByPeriodId(int nomineeId, int periodId);

        IEnumerable<QuestionModel> GetEOQQuestion();

        IEnumerable<QuestionModel> GetEOYQuestion();

        NominationModel GetEOQNominationById(int nominationId);

        NominationModel GetEOYNominationById(int nominationId);

        void ExportEOQ(IEnumerable<NominationViewModel> nominations, string periodName, string templatePath);

        void ExportEOY(IEnumerable<NominationViewModel> nominations, string periodName, string templatePath);

        IEnumerable<EOQYEmailModel> GetToSendEmail(ICriteria criteria);
    }
}