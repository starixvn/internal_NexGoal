namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Models.PRFeedback;

    public interface IPRFeedbackRepository
    {
        Task<IEnumerable<PRFeedbackModel>> GetPRFeedbacks(ICriteria criteria);
    }
}