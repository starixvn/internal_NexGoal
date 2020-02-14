namespace NexGoal.Models.Notification
{
    using System;
    using System.Xml.Serialization;
    using NexGoal.Core.Enumerations;
    using NexGoal.Core.Extensions;

    [Serializable]
    [XMLName("Row")]
    public class NotificationSetting
    {
        [XmlIgnore]
        public NotificationType Type { get; set; }

        public byte TypeId => (byte)Type;

        [XmlIgnore]
        public string Description { get; set; }

        public bool ReceiveNotification { get; set; }

        public bool ReceiveEmail { get; set; }

        public bool ReceiveSkypeMessage { get; set; }
    }
}