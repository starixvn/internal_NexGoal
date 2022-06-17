namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.PAFocus;

    public class PAFocusRepository : IPAFocusRepository
    {
        public async Task<IEnumerable<PAFocusModel>> GetPAFocusAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var pafcuses = await db.QueryAsync<PAFocusModel, Employee, PAFocusModel>(
                    (pafcus, employee) =>
                    {
                        pafcus.Employee = employee;
                        return pafcus;
                    },
                    "EmployeeId",
                    criteria);

                return pafcuses;
            }
        }
    }
}