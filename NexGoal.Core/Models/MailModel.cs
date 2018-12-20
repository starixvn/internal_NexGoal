namespace NexGoal.Core.Models
{
    using System.Collections.Generic;
    using MimeKit;

    public class MailModel
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ReceiverEmail { get; set; }
        public IEnumerable<string> Cc { get; set; } = new List<string>();
        public AttachmentCollection Attachments { get; set; } = new AttachmentCollection();
    }
}