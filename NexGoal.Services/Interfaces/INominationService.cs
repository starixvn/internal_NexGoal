namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Core.Model;
    using NexGoal.Criteria;
    using NexGoal.Model;

    public interface INominationService
    {
        NominatorSettingModel GetCurrentEOQByNominatorId(int nominatorId);

        NominatorSettingModel GetCurrentEOYByNominatorId(int nominatorId);

        IEnumerable<NominationViewModel> GetNominationByNominatorId(GetNominationByNominatorIdCriteria criteria);

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