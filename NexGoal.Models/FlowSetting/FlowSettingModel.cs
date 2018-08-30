namespace NexGoal.Models.FlowSetting
{
    using System;
    using System.Xml.Serialization;
    using NexGoal.Core.Extensions;

    [Serializable]
    [XMLName("Row")]
    public class FlowSettingModel
    {
        [XmlIgnore]
        public int GroupId { get; set; }

        [XmlIgnore]
        public int EmployeeId { get; set; }

        public int ProcessId { get; set; }
        public int Priority { get; set; }
        public int EvaluatorId { get; set; }

        [XmlIgnore]
        public string Code { get; set; }

        [XmlIgnore]
        public string NickName { get; set; }

        [XmlIgnore]
        public string FullName { get; set; }

        [XmlIgnore]
        public string Base64Image { get; set; }
    }
}