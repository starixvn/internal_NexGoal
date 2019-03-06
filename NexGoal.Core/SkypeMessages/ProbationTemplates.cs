namespace NexGoal.Core.SkypeMessages
{
    using System.Text;
    using Core.Constants;
    using Resources;

    public static class ProbationTemplates
    {
        public static string RemindHR(string nickName, string endDate)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Dear *{nickName}* {Constants.Newline}");
            message.Append($"The Probation process is going to be closed on *{endDate}*.{Constants.Newline}");
            message.Append($"{Message.SkypeMessage_PerformanceRecord_ProbationRemindHR}.{Constants.Newline}");
            message.Append($"Thank you {Constants.Newline}");
            message.Append($"NexGoal.");
            return message.ToString();
        }

        public static string RemindEvaluator(string nickName, string endDate, string appDomain)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Dear _{nickName}_ {Constants.Newline}");
            message.Append($"The Probation process is going to be closed on {endDate}.{Constants.Newline}");
            message.Append($"Please log in to {appDomain} to create a Performance Record (with a detailed comment, selected grade, and a `Probation` tag) for your member(s).{Constants.Newline}");
            message.Append($"Thank you {Constants.Newline}");
            message.Append($"NexGoal.");
            return message.ToString();
        }

        public static string RemindMember(string nickName, string endDate, string appDomain)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Dear _{nickName}_ {Constants.Newline}");
            message.Append($"The Probation process is going to be closed on {endDate}.{Constants.Newline}");
            message.Append($"Please log in to {appDomain} to create a Performance Record (with a detailed comment, selected grade, and a `Probation` tag) for yourself.{Constants.Newline}");
            message.Append($"Thank you {Constants.Newline}");
            message.Append($"NexGoal.");
            return message.ToString();
        }
    }
}