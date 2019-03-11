namespace NexGoal.Core
{
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services.Interfaces;
    using RestSharp;

    public class ChatbotService : IChatbotService
    {
        private readonly IRestClient webClient;
        private readonly IOptionsSnapshot<ChatbotConfigs> configuration;

        public ChatbotService(IOptionsSnapshot<ChatbotConfigs> configuration)
        {
            webClient = new RestClient { Encoding = Encoding.UTF8 };
            this.configuration = configuration;
        }

        public async Task SendMessageAsync(string conversationId, string message)
        {
            if (!string.IsNullOrWhiteSpace(conversationId) && configuration.Value.Enable)
            {
                var token = await GetToken().ConfigureAwait(false);
                await ForwardToBot(message, conversationId, token).ConfigureAwait(false);
            }
        }

        private async Task ForwardToBot(string message, string conversationId, ChatbotToken token)
        {
            webClient.BaseUrl = configuration.Value.BotServiceUrl;

            var request = new RestRequest("/messages/forward", Method.POST);
            request.AddHeader("Authorization", $"Bearer {token.AccessToken}");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("message", message);
            request.AddParameter("conversationId", conversationId);

            await webClient.ExecutePostTaskAsync(request).ConfigureAwait(false);
        }

        private async Task<ChatbotToken> GetToken()
        {
            webClient.BaseUrl = configuration.Value.TokenUrl;

            var request = new RestRequest(Method.POST);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", configuration.Value.MicrosoftAppId);
            request.AddParameter("client_secret", configuration.Value.MicrosoftAppPassword);
            request.AddParameter("scope", $"{configuration.Value.MicrosoftAppId}/.default");

            request.AddHeader("Content-type", "application/x-www-form-urlencoded");

            var response = await webClient.ExecutePostTaskAsync<ChatbotToken>(request).ConfigureAwait(false);

            return response.Data;
        }
    }
}