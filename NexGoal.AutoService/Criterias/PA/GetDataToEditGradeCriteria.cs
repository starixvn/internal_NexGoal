namespace NexGoal.AutoService.Criterias.PA
{
    using Fanex.Data.Repository;

    public class GetDataToEditGradeCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "Auto_PA_GetDataToUpdateGrade";
    }
}