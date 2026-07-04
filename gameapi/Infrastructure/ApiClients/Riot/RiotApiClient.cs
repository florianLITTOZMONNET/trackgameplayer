using Microsoft.Extensions.Configuration;


namespace Infrastructure.ApiClients.Riot;

public class RiotApiClient
{
    protected readonly HttpClient _http;
    protected readonly string _apiKey;

    protected RiotApiClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["Riot:ApiKey"] ?? throw new ArgumentException("Riot API Key is missing");
    }
}