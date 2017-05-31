namespace NexGoal.Services.Interfaces
{
    using Core.Model;
    using System.Collections.Generic;

    public interface IRecalculateScoreService
    {
        CommandResult RecalculateScore(int userId, IEnumerable<int> epaIds);
    }
}