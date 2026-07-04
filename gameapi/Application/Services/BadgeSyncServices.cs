// main.ts badge loop → BadgeSyncService.cs
using Infrastructure.ApiClients.Steam;
using Infrastructure.Database.Steam;

namespace Application.Services;

public class BadgeSyncService
{
    private readonly SteamBadgeClient _steamApi;
    private readonly SteamDatabaseManager _db;

    public BadgeSyncService(SteamBadgeClient steamApi, SteamDatabaseManager db)
    {
        _steamApi = steamApi;
        _db = db;
    }

    public async Task SyncPlayerBadgesAsync()
    {
        var players = _db.GetUnsearchedPlayers();
        if (players == null) { Console.WriteLine("No unsearched players."); return; }

        int totalAdded = 0;

        foreach (var player in players)
        {
            Console.WriteLine($"[{player.Id}] Fetching badges...");

            Core.Models.steam.GetBadgesResponse? badgeResponse;
            try { badgeResponse = await _steamApi.GetBadgeAsync(player.Id); }
            catch (Exception ex)
            {
                Console.WriteLine($"[{player.Id}] API error: {ex.Message}");
                continue;
            }

            if (badgeResponse?.Badges == null || badgeResponse.Badges.Count == 0)
            {
                Console.WriteLine($"[{player.Id}] No badges, skipping.");
                _db.MarkPlayerSearched(player.Id);
                continue;
            }

            foreach (var badge in badgeResponse.Badges)
            {
                var appid = badge.Appid?.ToString() ?? "1";
                var added = _db.AddBadge(
                    badge.Badgeid.ToString(),
                    appid,
                    badge.Communityitemid,
                    badge.Border_color,
                    badge.Scarcity
                );
                if (added) totalAdded++;
            }

            _db.MarkPlayerSearched(player.Id);
            Console.WriteLine($"[{player.Id}] Done. See you in 4 months.");
        }

        Console.WriteLine($"\nDone. New badges inserted: {totalAdded}");
    }
}