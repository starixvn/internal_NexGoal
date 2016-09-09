namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Core.Model;
    using NexGoal.Criteria;
    using NexGoal.Model;

    public interface IEOQService
    {
        IEnumerable<EOQYModel> GetEOQ(GetEOQCriteria criteria);

        EOQYModel GetEOQById(GetEOQByIdCriteria criteria);

        IEnumerable<EOQYNominatorSettingModel> GetEOQNominatorSettingById(GetEOQNominatorSettingByIdCriteria criteria);

        EOQYNominatorSettingModel GetCurrentEOQByNominatorId(int nominatorId);

        IEnumerable<EOQYNominationViewModel> GetEOQNominationByNominatorId(GetEOQNominationByNominatorIdCriteria criteria);

        CommandResult CreateEOQ(CreateEOQCriteria criteria);

        CommandResult UpdateEOQ(UpdateEOQCriteria criteria);

        CommandResult UpdateEOQNominatorSettingById(UpdateEOQNominatorSettingByIdCriteria criteria);

        CommandResult DeleteEOQ(DeleteEOQCriteria criteria);

        IEnumerable<EOQYQuestionModel> GetEOQQuestion();

        CommandResult CreateEOQNomination(CreateEOQNominationCriteria criteria);

        EOQYNominationModel GetEOQNominationById(int nominationId);

        CommandResult UpdateEOQNomination(UpdateEOQNominationCriteria criteria);

        CommandResult DeleteEOQNomination(DeleteEOQNominationCriteria criteria);

        CommandResult UpdateStatusEOQNomination(UpdateStatusEOQNominationCriteria criteria);
    }
}