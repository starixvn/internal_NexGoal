namespace NexGoal.BackgroundJob.Services;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Interfaces;
using Newtonsoft.Json;

#pragma warning disable S4462 // Calls to "async" methods should not be blocking
public class HttpClientService : IHttpClientService
{
    private static readonly HttpClient httpClient = CreateHttpClient();

    private static HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = ServerCertificateCustomValidationCallback;

        return new HttpClient(handler);
    }

    public T ExecuteGet<T>(string baseUrl, string token = null, IDictionary<string, object> data = null) where T : new()
        => SendHttpClient<T>(baseUrl, token, HttpMethod.Get, data);

    public T ExecutePost<T>(string baseUrl, string token, IDictionary<string, object> data) where T : new()
        => SendHttpClient<T>(baseUrl, token, HttpMethod.Post, data);

    public Task<T> ExecuteGetAsync<T>(string baseUrl, string token = null, IDictionary<string, object> data = null) where T : new()
        => SendHttpClientAsync<T>(baseUrl, token, HttpMethod.Get, data);

    public Task<T> ExecutePostAsync<T>(string baseUrl, string token, IDictionary<string, object> data = null) where T : new()
        => SendHttpClientAsync<T>(baseUrl, token, HttpMethod.Post, data);

    private static async Task<T> SendHttpClientAsync<T>(string baseUrl, string token, HttpMethod method, IDictionary<string, object> data)
       where T : new()
    {
        ValidateEndPoint(baseUrl);
        var url = BuildUrlWithQueryParams(baseUrl, method, data);
        using var request = CreateRequest(url, method, token);

        if (method == HttpMethod.Post && data != null)
        {
            var encodedData = JsonConvert.SerializeObject(data);
            var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
            request.Content = content;
        }

        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
        ThrowExceptionWhenNotSuccess(response);
        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseData))
            {
                return new T();
            }
            var result = JsonConvert.DeserializeObject<T>(responseData);
            return result;
        }

        return default(T);
    }

    private static T SendHttpClient<T>(string baseUrl, string token, HttpMethod method, IDictionary<string, object> data)
        where T : new()
    {
        ValidateEndPoint(baseUrl);
        var url = BuildUrlWithQueryParams(baseUrl, method, data);
        using var request = CreateRequest(url, method, token);

        if (method == HttpMethod.Post && data != null)
        {
            var encodedData = JsonConvert.SerializeObject(data);
            var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
            request.Content = content;
        }

        using var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None).Result;
        ThrowExceptionWhenNotSuccess(response);
        if (response.IsSuccessStatusCode)
        {
            var responseData = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(responseData);
            return result;
        }

        return default(T);
    }

    private static string BuildUrlWithQueryParams(string baseUrl, HttpMethod method, IDictionary<string, object> data)
    {
        if (method == HttpMethod.Get && data != null)
        {
            var queryString = string.Join("&", data.Select(x =>
                $"{HttpUtility.UrlEncode(x.Key)}={HttpUtility.UrlEncode(x.Value?.ToString())}"));
            return $"{baseUrl}?{queryString}";
        }

        return baseUrl;
    }

    private static HttpRequestMessage CreateRequest(string baseUrl, HttpMethod method, string apiKey)
    {
        var url = new Uri(baseUrl);
        var request = new HttpRequestMessage(method, url);
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (apiKey != null)
        {
            request.Headers.Add("X-API-KEY", apiKey);
        }

        return request;
    }

    private static void ValidateEndPoint(string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentNullException(nameof(baseUrl));
        }
    }

    private static void ThrowExceptionWhenNotSuccess(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var message = response.Content.ReadAsStringAsync().Result;
            var exception = new Exception(message);
            throw exception;
        }
    }

    private static bool ServerCertificateCustomValidationCallback(HttpRequestMessage message, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        => true;
}