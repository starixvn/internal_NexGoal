namespace NexGoal.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Models.Common;
    using NexGoal.Models.Discipline;

    public class DisciplineRepository : IDisciplineRepository
    {
        public async Task<IEnumerable<DisciplineModel>> GetDisciplineAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var disciplines = await db.QueryAsync<DisciplineModel, EmployeeBaseModel, EmployeeBaseModel, DisciplineModel>(
                    (discipline, employee, creator) =>
                    {
                        discipline.Employee = employee;
                        discipline.Creator = creator;

                        return discipline;
                    },
                    "EmployeeId, EmployeeId",
                    criteria).ConfigureAwait(false);

                return disciplines;
            }
        }
    }
}