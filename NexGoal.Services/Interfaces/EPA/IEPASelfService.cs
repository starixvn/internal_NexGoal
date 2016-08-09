using NexGoal.Criteria.EmployeeEPA;
using NexGoal.Model.EmployeeEPA;
using System.Collections.Generic;

namespace NexGoal.Services.Interfaces
{
    public interface IEPASelfService
    {
        IEnumerable<EPAModel> GetEPA(GetEPAByEmployeeCriteria criteria);

        EPAModel GetEPAInfo(GetEPAInfoByEmployeeCriteria criteria);
    }
}
