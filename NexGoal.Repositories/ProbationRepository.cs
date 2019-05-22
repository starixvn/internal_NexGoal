namespace NexGoal.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Models.Probation;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ProbationRepository : IProbationRepository
    {
        public async Task<IEnumerable<ProbationEvaluationModel>> GetProbationersAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    var probationers = await multi.ReadAsync<ProbationEvaluationModel>();

                    var settings = await multi.ReadAsync<ProbationSettingModel>();

                    foreach (var probationer in probationers)
                    {
                        probationer.Settings = settings.Where(x => x.EmployeeId == probationer.EmployeeId);
                    }

                    return probationers;
                }
            }
        }
    }
}