namespace NexGoal.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NexGoal.Models.PRFeedback;

    public interface IPRFeedbackRepository
    {
        Task<PRFeedbackModel> GetPRFeedbacks(ICriteria criteria);
    }
}