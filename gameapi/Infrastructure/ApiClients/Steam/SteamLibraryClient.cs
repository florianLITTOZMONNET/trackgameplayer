using System.Net.Http.Json;
using Core.Models.steam;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.ApiClients.Steam;

public class SteamLibraryClient : SteamApiClient
{
    public SteamLibraryClient(HttpClient http, IConfiguration config) 
        : base(http, config) { }

    public async Task<LibraryStats?> GetLibraryAsync(string steamId)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var url =$"http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={_apiKey}&steamid={steamId}&include_appinfo=true&format=json";
        var response = await _http.GetFromJsonAsync<LibraryWrapper>(url, cts.Token);
        return response?.Response;

    }
}