using NexGoal.Criteria.EmployeeEPA;
using NexGoal.Model.EmployeeEPA;
using System.Collections.Generic;

namespace NexGoal.Services.Interfaces
{
    public interface IEPAReviewerService
    {
        IEnumerable<EPAModel> GetEPA(GetEPAByReviewerCriteria criteria);

        EPAModel GetEPAInfo(GetEPAInfoByReviewerCriteria criteria);
    }
}
