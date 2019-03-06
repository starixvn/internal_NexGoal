namespace NexGoal.Core.Models
{
    using System;

    public class ChatbotConfigs
    {
        public Uri BotServiceUrl { get; set; } = new Uri("https://nexbot.tk/api/");

        public string MicrosoftAppId { get; set; } = "1fca3d04-4799-4544-8744-89367a838238";

        public string MicrosoftAppPassword { get; set; } = "NJT594-!uoobpdkYYBF29:^";

        public Uri TokenUrl { get; set; }
    }
}