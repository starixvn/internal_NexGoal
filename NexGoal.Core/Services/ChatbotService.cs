namespace NexGoal.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;

    public class ChatbotService(
        ChatbotConfigs chatbotConfigs,
        IHttpClientFactory httpClientFactory) : IChatbotService
    {
        private readonly HttpClient httpClient = httpClientFactory.CreateClient();

        public async Task SendMessageAsync(string conversationId, string message)
        {
            if (chatbotConfigs.Enable && !string.IsNullOrWhiteSpace(conversationId))
            {
                var request = CreateRequestAsync("messages/forward", HttpMethod.Post);
                var data = new Dictionary<string, string>
                {
                    { "conversationId", conversationId },
                    { "message", message }
                };

                request.Content = new FormUrlEncodedContent(data);

                using var response = await httpClient.SendAsync(request);
            }
        }


        private HttpRequestMessage CreateRequestAsync(string endPointPath, HttpMethod method)
        {
            Uri url = new($"{chatbotConfigs.BotServiceUrl}{endPointPath}");
            var request = new HttpRequestMessage(method, url);
            return request;
        }
    }
}