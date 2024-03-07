namespace NexGoal.Core.Models
{
    using System;

    public class ChatbotConfigs
    {
        public bool Enable { get; set; }

        public Uri BotServiceUrl { get; set; } = new Uri("https://hrbot.nexdev.net/api/");
    }
}