namespace NexGoal.Core.SkypeMessages
{
    using System.Text;
    using Core.Constants;
    using NexGoal.Resources;

    public static class PATemplates
    {
        public static string RemindStartEvaluationToSelf(string nickName, string periodName, string startDate, string appDomain)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Dear, *{nickName}* {Constants.Newline}");
            message.Append($"The Performance Appraisal for the *{periodName}* will begin on {startDate} {Constants.Newline}");
            message.Append(Message.SkypeTemplate_PA_RemindStartEvaluationToSelf);
            message.Append(Constants.Newline);
            message.Append($"Please log in to _{appDomain}_ to create a Performance Record. {Constants.Newline}");
            message.Append($"Thank you, {Constants.Newline}");
            message.Append("NexGoal");
            return message.ToString();
        }

        public static string RemindStartEvaluationToSupervisor(string nickName, string periodName, string startDate, string appDomain)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Dear, *{nickName}* {Constants.Newline}");
            message.Append($"The Performance Appraisal for the *{periodName}* will begin on {startDate} {Constants.Newline}");
            message.Append(Message.SkypeTemplate_PA_RemindStartEvaluationToSupervisor);
            message.Append(Constants.Newline);
            message.Append($"Please log in to _{appDomain}_ to create a Performance Record. {Constants.Newline}");
            message.Append($"Thank you, {Constants.Newline}");
            message.Append("NexGoal");
            return message.ToString();
        }

        public static string Start(string nickName, string periodName, string appDomain)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Dear, *{nickName}* {Constants.Newline}");
            message.Append($"The Performance Appraisal for the *{periodName}* is in progress. {Constants.Newline}");
            message.Append($"Please log in to _{appDomain}_ to revise and submit the PA result for your members. {Constants.Newline}");
            message.Append($"Thank you, {Constants.Newline}");
            message.Append("NexGoal");
            return message.ToString();
        }

        public static string Remind(string nickName, string periodName, string endDate, string appDomain)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"*Dear, *{nickName}* {Constants.Newline}");
            message.Append($"The Performance Appraisal for the *{periodName}* is going to be closed on {endDate}. {Constants.Newline}");
            message.Append($"Please log in to _{appDomain}_ to revise and submit the PA result for your members. {Constants.Newline}");
            message.Append($"Thank you, {Constants.Newline}");
            message.Append("NexGoal");
            return message.ToString();
        }

        public static string EndEvaluation(string nickName, string periodName, string appDomain)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Dear, *{nickName}* {Constants.Newline}");
            message.Append($"The Performance Appraisal for the {periodName} is now complete. {Constants.Newline}");
            message.Append($"Please log in to _{appDomain}_ to view your final result. {Constants.Newline}");
            message.Append($"Thank you, {Constants.Newline}");
            message.Append("NexGoal");
            return message.ToString();
        }
    }
}