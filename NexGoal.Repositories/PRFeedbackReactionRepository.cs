namespace NexGoal.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.PRFeedbackReaction;

    public class PRFeedbackReactionRepository : IPRFeedbackReactionRepository
    {
        public async Task<PRFeedbackReactionModel> GetPRFeedbackReactionByFeedbackId(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var reactions = await db.QueryEntityAsync<PRFeedbackReactionModel, Employee, PRFeedbackReactionModel>(
                    (reaction, creator) =>
                    {
                        reaction.Creator = creator;
                        return reaction;
                    },
                    "EmployeeId", criteria);

                return reactions;
            }
        }
    }
}