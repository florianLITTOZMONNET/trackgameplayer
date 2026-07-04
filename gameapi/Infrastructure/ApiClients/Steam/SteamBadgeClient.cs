using System.Net.Http.Json;
using Core.Models.steam;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.ApiClients.Steam;

public class SteamBadgeClient : SteamApiClient
{
    public SteamBadgeClient(HttpClient httpClient, IConfiguration config)
        :base(httpClient, config){}

    public async Task<GetBadgesResponse?> GetBadgeAsync(string steamId)
    {
        var url = $"https://api.steampowered.com/IPlayerService/GetBadges/v1/?key={_apiKey}&steamid={steamId}&format=json";
        var response = await _http.GetFromJsonAsync<BadgesWrapper>(url);
        return response?.Response;
    }
    
}