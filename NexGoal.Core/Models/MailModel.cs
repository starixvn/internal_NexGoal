namespace NexGoal.Core.Models
{
    using MimeKit;

    public class MailModel
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ReceiverEmail { get; set; }
        public AttachmentCollection Attachments { get; set; } = new AttachmentCollection();
    }
}