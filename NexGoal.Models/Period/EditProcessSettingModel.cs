namespace NexGoal.Models.Period
{
    using System.Xml.Serialization;
    using NexGoal.Core.Extensions;

    [XMLName("Row")]
    public class EditProcessSettingModel
    {
        public byte ProcessId { get; set; }

        public int PeriodId { get; set; }

        public int Duration { get; set; }

        public bool NoticeAtBegin { get; set; }

        public int RemindDay { get; set; }

        [XmlIgnore]
        public int? FocusDay { get; set; }

        [XMLName("FocusDay")]
        public string XmlFocusDay
        {
            get => FocusDay.HasValue ? FocusDay.Value.ToString() : string.Empty;
        }
    }
}