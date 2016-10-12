namespace NexGoal.Services.Interfaces
{
    using NexGoal.Core.Model;
    using NexGoal.Criteria;
    using NexGoal.Model;
    using System.Collections.Generic;

    public interface INominationService
    {
        NominatorSettingModel GetLatestEOQByNominatorId(int nominatorId);

        NominatorSettingModel GetLatestEOYByNominatorId(int nominatorId);

        IEnumerable<NominationViewModel> GetNominationByNominatorId(GetNominationByNominatorIdCriteria criteria);

        IEnumerable<NominationEPAModel> GetNominationByPeriodId(GetNominationByPeriodIdCriteria criteria);

        IEnumerable<QuestionModel> GetEOQQuestion();

        IEnumerable<QuestionModel> GetEOYQuestion();

        CommandResult CreateEOQNomination(CreateEOQNominationCriteria criteria);

        CommandResult CreateEOYNomination(CreateEOYNominationCriteria criteria);

        NominationModel GetEOQNominationById(int nominationId);

        NominationModel GetEOYNominationById(int nominationId);

        CommandResult UpdateNomination(UpdateNominationCriteria criteria);

        CommandResult DeleteNomination(DeleteNominationCriteria criteria);

        CommandResult UpdateStatusNomination(UpdateStatusNominationCriteria criteria);

        void ExportEOQ(IEnumerable<NominationViewModel> nominations, string periodName, string templatePath);

        void ExportEOY(IEnumerable<NominationViewModel> nominations, string periodName, string templatePath);
    }
}