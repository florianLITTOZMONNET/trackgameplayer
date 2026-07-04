using System.Net;
using Infrastructure.ApiClients.Steam;
using Infrastructure.Database.Steam;

namespace Application.Services;

public class PlayerSyncService
{
    private readonly SteamLibraryClient _steamLibraryApi;
    private readonly SteamDatabaseManager _db;

    public PlayerSyncService(SteamLibraryClient steamApi, SteamDatabaseManager db)
    {
        _steamLibraryApi = steamApi;
        _db = db;
    }

    public async Task SyncPlayerLibrariesAsync()
    {
        var players = _db.GetUnsearchedPlayers();
        if (players == null) { Console.WriteLine("No unsearched players."); return; }

        for (int i = 0; i < players.Count; i++)
        {
            var id = players[i].Id;
            Console.WriteLine($"Processing: {id}");

            try
            {
                var library = await _steamLibraryApi.GetLibraryAsync(id);

                if (library == null || library.Games.Count == 0)
                {
                    Console.WriteLine($"[{id}] Empty/private, skipping.");
                    _db.MarkPlayerSearched(id);
                }
                else
                {
                    foreach (var game in library.Games)
                    {
                        _db.AddGame(game.Appid.ToString(), game.Name);
                        _db.linkGame(id, game.Appid.ToString());
                    }

                    _db.MarkPlayerSearched(id);
                    Console.WriteLine($"[{id}] Done. {library.Games.Count} games.");
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"[{id}] Timed out, skipping.");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                Console.WriteLine($"[{id}] Rate limited! Waiting 60s...");
                await Task.Delay(60_000);
                i--; // retry same player
                continue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{id}] Error: {ex.Message}");
            }

            await Task.Delay(1_500);
        }
    }
}