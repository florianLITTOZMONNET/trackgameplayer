using System.Net.Http.Json;
using Core.Models.steam;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.ApiClients.Steam;

public class SteamPlayerClient : SteamApiClient
{
    public SteamPlayerClient(HttpClient http, IConfiguration config)
        : base(http, config){}

    public async Task<List<SteamPlayer>> GetPlayerBasicInfoAsync(string steamId)
    {
        var url=$"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={_apiKey}&steamids={steamId}";
        var response = await _http.GetFromJsonAsync<GetPlayerSummariesResponse>(url);
        return response?.Response?.Players ?? [];
    }
}