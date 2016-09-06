namespace NexGoal.Services.Interfaces
{
    using System.Collections.Generic;
    using NexGoal.Core.Model;
    using NexGoal.Criteria;
    using NexGoal.Model;

    public interface IEOQService
    {
        IEnumerable<EOQModel> GetEOQ(GetEOQCriteria criteria);

        CommandResult CreateEOQ(CreateEOQCriteria criteria);

        CommandResult UpdateEOQ(UpdateEOQCriteria criteria);

        CommandResult DeleteEOQ(DeleteEOQCriteria criteria);
    }
}