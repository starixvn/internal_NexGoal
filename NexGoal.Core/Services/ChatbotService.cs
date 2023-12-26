namespace NexGoal.Core.Services
{
    using System.Text;
    using System.Threading.Tasks;
    using NexGoal.Core.Models;
    using RestSharp;

    public class ChatbotService : IChatbotService
    {
        private readonly IRestClient webClient;
        private readonly ChatbotConfigs chatbotConfigs;

        public ChatbotService(ChatbotConfigs chatbotConfigs)
        {
            webClient = new RestClient { Encoding = Encoding.UTF8 };
            this.chatbotConfigs = chatbotConfigs;
        }

        public async Task SendMessageAsync(string conversationId, string message)
        {
            if (!string.IsNullOrWhiteSpace(conversationId) && chatbotConfigs.Enable)
            {
                ChatbotToken token = await GetTokenAsync();
                await ForwardToBotAsync(message, conversationId, token);
            }
        }

        private async Task ForwardToBotAsync(string message, string conversationId, ChatbotToken token)
        {
            webClient.BaseUrl = chatbotConfigs.BotServiceUrl;

            var request = new RestRequest("/messages/forward", Method.POST);
            request.AddHeader("Authorization", $"Bearer {token.AccessToken}");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("message", message);
            request.AddParameter("conversationId", conversationId);

            await webClient.ExecutePostAsync(request);
        }

        private async Task<ChatbotToken> GetTokenAsync()
        {
            webClient.BaseUrl = chatbotConfigs.TokenUrl;

            var request = new RestRequest(Method.POST);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", chatbotConfigs.MicrosoftAppId);
            request.AddParameter("client_secret", chatbotConfigs.MicrosoftAppPassword);
            request.AddParameter("scope", $"{chatbotConfigs.MicrosoftAppId}/.default");

            request.AddHeader("Content-type", "application/x-www-form-urlencoded");

            var response = await webClient.ExecutePostAsync<ChatbotToken>(request);

            return response.Data;
        }
    }
}