namespace NexGoal.Core.SkypeMessages
{
    using System.Text;
    using NexGoal.Core.Constants;

    public static class PerformanceRecordTemplates
    {
        public static string CreatePRMemberToSuperVisor(string reciverNickName, string actorNickName, string appDomain)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"*Dear, {reciverNickName}* {Constants.Newline}");
            message.Append($"*{actorNickName}* has created a Performance Record for him/herself. {Constants.Newline}");
            message.Append($"Please log in to _{appDomain}_ to view the details. {Constants.Newline}");
            message.Append("Thank you");
            return message.ToString();
        }

        public static string CreatePRSupervisorToMember(string reciverNickName, string actorNickName, string appDomain)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"*Dear, {reciverNickName}* {Constants.Newline}");
            message.Append($"*{actorNickName}* has created a Performance Record for you. {Constants.Newline}");
            message.Append($"Please log in to _{appDomain}_ to view the details. {Constants.Newline}");
            message.Append("Thank you");
            return message.ToString();
        }

        public static string CreatePRSupervisorToSupervisor(string reciverNickName,
            string actorNickName,
            string employeeName,
            string appDomain)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"*Dear, {reciverNickName}* {Constants.Newline}");
            message.Append($"*{actorNickName}* has created a Performance Record for {employeeName}. {Constants.Newline}");
            message.Append($"Please log in to _{appDomain}_ to view the details. {Constants.Newline}");
            message.Append("Thank you");
            return message.ToString();
        }

        public static string EditPRMemberToSuperVisor(string reciverNickName, string actorNickName, string appDomain)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"*Dear, {reciverNickName}* {Constants.Newline}");
            message.Append($"*{actorNickName}* has edited his/her Performance Record. {Constants.Newline}");
            message.Append($"Please log in to _{appDomain}_ to view the details. {Constants.Newline}");
            message.Append("Thank you");
            return message.ToString();
        }

        public static string EditPRSupervisorToMember(string reciverNickName, string actorNickName, string appDomain)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"*Dear, {reciverNickName}* {Constants.Newline}");
            message.Append($"*{actorNickName}* has edited his/her Performance Record for you {Constants.Newline}");
            message.Append($"Please log in to _{appDomain}_ to view the details. {Constants.Newline}");
            message.Append("Thank you");
            return message.ToString();
        }

        public static string EditPRSupervisorToSupervisor(string reciverNickName,
            string actorNickName,
            string employeeName,
            string appDomain)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"*Dear, {reciverNickName}* {Constants.Newline}");
            message.Append($"*{actorNickName}* has edited his/her Performance Record for {employeeName}. {Constants.Newline}");
            message.Append($"Please log in to _{appDomain}_ to view the details. {Constants.Newline}");
            message.Append("Thank you");
            return message.ToString();
        }
    }
}