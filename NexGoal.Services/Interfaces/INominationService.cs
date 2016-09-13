namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using System.IO;
    using NexGoal.Core.Model;
    using NexGoal.Criteria;
    using NexGoal.Model;

    public interface INominationService
    {
        NominatorSettingModel GetCurrentEOQByNominatorId(int nominatorId);

        IEnumerable<NominationViewModel> GetEOQNominationByNominatorId(GetEOQNominationByNominatorIdCriteria criteria);

        IEnumerable<QuestionModel> GetEOQQuestion();

        CommandResult CreateEOQNomination(CreateEOQNominationCriteria criteria);

        NominationModel GetEOQNominationById(int nominationId);

        CommandResult UpdateEOQNomination(UpdateEOQNominationCriteria criteria);

        CommandResult DeleteEOQNomination(DeleteEOQNominationCriteria criteria);

        CommandResult UpdateStatusEOQNomination(UpdateStatusEOQNominationCriteria criteria);

        MemoryStream ExportEOQ(IEnumerable<NominationViewModel> nominations, string periodName);
    }
}