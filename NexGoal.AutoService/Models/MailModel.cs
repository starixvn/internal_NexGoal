namespace NexGoal.AutoService.Models
{
    using System.Collections.Generic;
    using System.Net.Mail;

    public class MailModel
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ReceiverEmail { get; set; }
        public IList<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}