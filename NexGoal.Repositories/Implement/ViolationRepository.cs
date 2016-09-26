using Fanex.Data;
using Fanex.Data.Repository;
using NexGoal.Model.Enitites;
using NexGoal.Model.Integration;
using NexGoal.Model.Violation;
using NexGoal.Repositories.Interfaces;
using System.Collections.Generic;

namespace NexGoal.Repositories.Implement
{
    public class ViolationRepository : IViolationRepository
    {
        public IEnumerable<ViolationViewModel> Get(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<ViolationViewModel>(criteria.GetSpParams());
            }
        }

        public ViolationViewModel GetById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.QueryEntity<ViolationViewModel>(criteria.GetSpParams());
            }
        }
    }
}
