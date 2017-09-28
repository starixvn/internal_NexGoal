namespace NexGoal.BackgroundService
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using NexGoal.Model.Period;
    using System.Linq;

    public class EvaluationRepository : IEvaluationRepository
    {
        public AutoEmployeePeriodModel GetEmployeeProbationEvaluation(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var employees = multi.Read<int>();
                    var periods = multi.Read<int>();
                    var employeePeriod = new AutoEmployeePeriodModel();

                    if (periods != null && periods.Any() && employees != null && employees.Any())
                    {
                        employeePeriod.PeriodId = periods.First();
                        employeePeriod.EmployeeIds = string.Join(",", employees);
                    }

                    return employeePeriod;
                }
            }
        }
    }
}