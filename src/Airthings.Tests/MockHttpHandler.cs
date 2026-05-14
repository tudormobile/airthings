using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Airthings.Tests;

[ExcludeFromCodeCoverage]
public class MockHttpMessageHandler : HttpMessageHandler
{
    public string? ProvidedClientSecret { get; private set; }
    public string? ProvidedClientId { get; private set; }
    public string? AuthUpdatedToken { get; set; }
    public string? TokenJsonResponse { get; set; }
    public Uri? ProvidedRequestUri { get; set; }
    public Exception? AlwaysThrows { get; set; } = null;
    public HttpResponseMessage? AlwaysResponds { get; set; } = null;
    public string? JsonResponse { get; set; }
    public string? JsonSecondaryResponse { get; set; }
    public bool ForceNullResponse { get; set; } = false;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ProvidedRequestUri = request.RequestUri;
        return await Task.Run(() =>
        {
            if (request.Method == HttpMethod.Post && AuthUpdatedToken != null)
            {
                using var reader = new StreamReader(request.Content!.ReadAsStream());
                var payload = reader.ReadToEnd();
                using var doc = JsonDocument.Parse(payload);
                ProvidedClientId = doc.RootElement.GetProperty("client_id").ToString();
                ProvidedClientSecret = doc.RootElement.GetProperty("client_secret").ToString();
                var json = TokenJsonResponse ?? $"{{\"access_token\": \"{AuthUpdatedToken}\"}}";
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
                };
            }
            if (AuthUpdatedToken != null && ProvidedClientSecret == null && ProvidedClientId == null)
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
            if (AlwaysResponds != null)
            {
                if (JsonResponse != null)
                {
                    var json = JsonResponse;
                    JsonResponse = JsonSecondaryResponse ?? JsonResponse;
                    JsonSecondaryResponse = null;
                    AlwaysResponds.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                }
                return AlwaysResponds;
            }
            if (AlwaysThrows != null)
            {
                throw AlwaysThrows;
            }
            if (JsonResponse != null)
            {
                var json = JsonResponse;
                JsonResponse = JsonSecondaryResponse ?? JsonResponse;
                JsonSecondaryResponse = null;
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
                };
            }
            if (ForceNullResponse)
            {
                return null!;
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
        });
    }
}
