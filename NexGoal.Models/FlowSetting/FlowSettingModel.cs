namespace NexGoal.Models.FlowSetting
{
    using System;
    using System.Xml.Serialization;
    using NexGoal.Core.Extensions;

    [Serializable]
    [XMLName("Row")]
    public class FlowSettingModel
    {
        public int ProcessId { get; set; }
        public int Priority { get; set; }
        public int EvaluatorId { get; set; }

        [XmlIgnore]
        public string Code { get; set; }

        [XmlIgnore]
        public string NickName { get; set; }

        [XmlIgnore]
        public string FullName { get; set; }
    }
}