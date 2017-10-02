namespace NexGoal.Criteria.Dashboard
{
    using Fanex.Data.Repository;

    public class GetToDoListCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "Dashboard_GetToDoList";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}