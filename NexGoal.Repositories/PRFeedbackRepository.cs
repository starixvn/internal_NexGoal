namespace NexGoal.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.PRFeedback;
    using NexGoal.Models.PRFeedbackReaction;

    public class PRFeedbackRepository : IPRFeedbackRepository
    {
        public async Task<PRFeedbackModel> GetPRFeedback(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var feedbacks = multi.Read<PRFeedbackModel, Employee, PRFeedbackModel>(
                        (feedback, creator) =>
                        {
                            feedback.Creator = creator;

                            return feedback;
                        }, "EmployeeId");

                    var feedbackReactions = multi.Read<PRFeedbackReactionModel, Employee, PRFeedbackReactionModel>(
                        (reaction, creator) =>
                        {
                            reaction.Creator = creator;

                            return reaction;
                        }, "EmployeeId");

                    foreach (var feedback in feedbacks)
                    {
                        feedback.Reactions = feedbackReactions.Where(reaction => reaction.PRFeedbackId == feedback.PRFeedbackId);
                    }

                    return feedbacks.FirstOrDefault();
                }
            }
        }
    }
}