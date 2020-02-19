namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.PRReaction;

    public class PRReactionRepository : IPRReactionRepository
    {
        public async Task<IEnumerable<PRReactionModel>> GetPRReactionByPRId(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var reactions = await db.QueryAsync<PRReactionModel, Employee, PRReactionModel>(
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