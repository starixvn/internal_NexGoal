namespace NexGoal.Core.SkypeMessages
{
    using System.Text;
    using Core.Constants;

    public static class NominationTemplates
    {
        public static string StartToNominator(string nickName, string periodTitle, string appDomain)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Dear, *{nickName}* {Constants.Newline}");
            message.Append($"The Nomination {periodTitle} process is in progress. {Constants.Newline}");
            message.Append($"Please log in to _{appDomain}_ to make a vote for your nominee(s). {Constants.Newline}");
            message.Append($"Thank you, {Constants.Newline}");
            message.Append("NexGoal");
            return message.ToString();
        }

        public static string RemindToNominator(string nickName, string periodTitle, string endDate, string appDomain)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Dear, *{nickName}* {Constants.Newline}");
            message.Append($"The Nomination {periodTitle} process is going to be closed on {endDate}. {Constants.Newline}");
            message.Append($"Please log in to _{appDomain}_ to make a vote for your nominee(s). {Constants.Newline}");
            message.Append($"Thank you, {Constants.Newline}");
            message.Append("NexGoal");
            return message.ToString();
        }
    }
}