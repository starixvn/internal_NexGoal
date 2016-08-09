using NexGoal.Criteria.EmployeeEPA;
using NexGoal.Model.EmployeeEPA;
using System.Collections.Generic;

namespace NexGoal.Services.Interfaces
{
    public interface IEPAEvaluatorService
    {
        IEnumerable<EPAModel> GetEPA(GetEPAByEvaluatorCriteria criteria);

        EPAModel GetEPAInfo(GetEPAInfoByEvaluatorCriteria criteria);
    }
}
