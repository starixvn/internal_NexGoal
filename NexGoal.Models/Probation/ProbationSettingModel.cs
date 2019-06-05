namespace NexGoal.Models.Probation
{
    using NexGoal.Core.Extensions;
    using System;
    using System.Xml.Serialization;

    [XMLName("Row")]
    public class ProbationSettingModel
    {
        public int EmployeeId { get; set; }

        public int ProcessId { get; set; }

        public int EvaluatorId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        [XmlIgnore]
        public string EvaluatorNickname { get; set; }
    }
}