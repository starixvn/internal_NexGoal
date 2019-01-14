namespace NexGoal.AutoService.Models.PA
{
    using System;
    using NexGoal.Core.Extensions;

    [Serializable]
    [XMLName("Row")]
    public class PARecordModel
    {
        public int PARecordId { get; set; }
        public string GradeId { get; set; }
        public int ToeicPolicy { get; set; }
        public int StatusId { get; set; }
    }
}