using Microsoft.Extensions.Configuration;

namespace Infrastructure.ApiClients.Steam;

public class SteamApiClient
{
    protected readonly HttpClient _http;
    protected readonly string _apiKey;

    protected SteamApiClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["Steam:ApiKey"] ?? throw new ArgumentException("Steam API Key is missing");
    }
}