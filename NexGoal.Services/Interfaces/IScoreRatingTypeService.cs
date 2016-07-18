namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Criteria.ScoreRatingType;
    using NexGoal.Model.ScoreRatingType;

    public interface IScoreRatingTypeService
    {
        IEnumerable<ScoreRatingTypeModel> GetScoreRatingType(GetScoreRatingTypeCriteria criteria);
    }
}