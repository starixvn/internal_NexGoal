namespace NexGoal.Services.Interfaces
{
    using NexGoal.Criteria.ScoreRatingType;
    using NexGoal.Model.ScoreRatingType;
    using System.Collections.Generic;

    public interface IScoreRatingTypeService
    {
        IEnumerable<ScoreRatingTypeModel> GetScoreRatingType(GetScoreRatingTypeCriteria criteria);
    }
}