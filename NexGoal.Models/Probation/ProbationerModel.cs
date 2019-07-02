namespace NexGoal.Models.Probation
{
    using System.Collections.Generic;

    public class ProbationerModel : BaseProbationerModel
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public IEnumerable<ProbationSettingModel> Settings { get; set; }
    }
}