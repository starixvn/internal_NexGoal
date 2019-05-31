namespace NexGoal.AutoService.Criterias.Probation
{
    using NexGoal.Core.Criteria;

    public class AddNewEmployeeToProbationCriteria : CommandBaseCriteria
    {
        public override string GetSettingKey() => "Auto_Probation_InsertNewEmployee";

        public override bool IsValid() => true;
    }
}