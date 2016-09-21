using Fanex.Data;
using Fanex.Data.Repository;
using NexGoal.Model.Enitites;
using NexGoal.Model.Integration;
using NexGoal.Repositories.Interfaces;
using System.Collections.Generic;

namespace NexGoal.Repositories.Implement
{
    public class IntegrationRepository : IIntegrationRepository
    {
        public IEnumerable<EmployeeWorkingViewModel> GetEmployeeWorking(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<EmployeeWorkingViewModel>(criteria.GetSpParams());
            }
        }
    }
}
