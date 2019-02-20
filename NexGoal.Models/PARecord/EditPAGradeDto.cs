namespace NexGoal.Models.PARecord
{
    using System;
    using NexGoal.Core.Extensions;

    [Serializable]
    [XMLName("Row")]
    public class EditPAGradeDto
    {
        public int PARecordId { get; set; }
        public string OriginalGradeId { get; set; }
        public string GradeId { get; set; }
        public int ToeicPolicy { get; set; }
        public int StatusId { get; set; }
    }
}