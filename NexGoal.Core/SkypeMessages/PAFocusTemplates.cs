namespace NexGoal.Core.SkypeMessages
{
    using System.Text;
    using Core.Constants;

    public static class PAFocusTemplates
    {
        public static string StartToEvaluator(string nickName, string periodName, string appDomain)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Dear, *{nickName}* {Constants.Newline}");
            message.Append($"The PA Focus for the {periodName} is in progress. {Constants.Newline}");
            message.Append($"Please log in to _{appDomain}_ to set the objectives for your team members. {Constants.Newline}");
            message.Append($"Thank you, {Constants.Newline}");
            message.Append("NexGoal");
            return message.ToString();
        }

        public static string RemindToEvaluator(string nickName, string periodName, string endDate, string appDomain)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Dear, *{nickName}* {Constants.Newline}");
            message.Append($"The PA Focus for the {periodName} is going to be closed on {endDate}. {Constants.Newline}");
            message.Append($"Please log in to _{appDomain}_ to review the objectives for your team members. {Constants.Newline}");
            message.Append($"Thank you, {Constants.Newline}");
            message.Append("NexGoal");
            return message.ToString();
        }
    }
}