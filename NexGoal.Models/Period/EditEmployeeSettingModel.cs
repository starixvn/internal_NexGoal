namespace NexGoal.Models.Period
{
    using System;
    using System.Xml.Serialization;
    using NexGoal.Core.Extensions;

    [XMLName("Row")]
    public class EditEmployeeSettingModel
    {
        public byte ProcessId { get; set; }

        [XmlIgnore]
        public int? EvaluatorId { get; set; }

        [XMLName("EvaluatorId")]
        public string XmlEvaluatorId => EvaluatorId.HasValue ? EvaluatorId.Value.ToString() : string.Empty;

        [XmlIgnore]
        public DateTime? StartDate { get; set; }

        [XMLName("StartDate")]
        public string XmlStartDate => StartDate.HasValue ? StartDate.Value.ToString("yyyy-MM-dd") : string.Empty;

        [XmlIgnore]
        public DateTime? EndDate { get; set; }

        [XMLName("EndDate")]
        public string XmlEndDate => EndDate.HasValue ? EndDate.Value.ToString("yyyy-MM-dd") : string.Empty;
    }
}