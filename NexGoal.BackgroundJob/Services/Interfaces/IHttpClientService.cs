namespace NexGoal.BackgroundJob.Services.Interface;

public interface IHttpClientService
{
    T ExecuteGet<T>(string baseUrl, string apiKey = null, IDictionary<string, object> data = null) where T : new();

    T ExecutePost<T>(string baseUrl, string apiKey, IDictionary<string, object> data) where T : new();

    Task<T> ExecuteGetAsync<T>(string baseUrl, string apiKey = null, IDictionary<string, object> data = null) where T : new();

    Task<T> ExecutePostAsync<T>(string baseUrl, string token, IDictionary<string, object> data = null) where T : new();
}