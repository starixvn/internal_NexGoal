namespace NexGoal.Services.Implement
{
    using NexGoal.Core.Services;
    using NexGoal.Criteria.ScoreRatingType;
    using NexGoal.Model.ScoreRatingType;
    using NexGoal.Services.Interfaces;
    using System.Collections.Generic;

    public class ScoreRatingTypeService : BaseService, IScoreRatingTypeService
    {
        public IEnumerable<ScoreRatingTypeModel> GetScoreRatingType(GetScoreRatingTypeCriteria criteria)
        {
            IEnumerable<ScoreRatingTypeModel> scoreRatingTypes = new List<ScoreRatingTypeModel>();

            if (criteria != null && criteria.IsValid())
            {
                scoreRatingTypes = GetData<ScoreRatingTypeModel>(criteria);
            }

            return scoreRatingTypes;
        }
    }
}