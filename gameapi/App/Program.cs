using Application.Services;
using Infrastructure.ApiClients.Steam;
using Infrastructure.Database;
using Infrastructure.Database.Steam;
using Infrastructure.WebHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();

services.AddSingleton<IConfiguration>(config);
services.AddHttpClient<SteamPlayerClient>();
services.AddHttpClient<SteamLibraryClient>();
services.AddHttpClient<SteamBadgeClient>();
services.AddHttpClient<ok2>();        
var solutionDir = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "../../../..") 
);
var dbPath = Path.Combine(solutionDir, "my_steam_data.db");
services.AddSingleton(_ => new SteamDatabaseManager(dbPath));
services.AddTransient<PlayerSyncService>();
services.AddTransient<BadgeSyncService>();
services.AddTransient<ok2>();           

var provider = services.BuildServiceProvider();

// Menu 
Console.WriteLine("Choose an action:");
Console.WriteLine("  1 = Sync Libraries");
Console.WriteLine("  2 = Sync Badges");
Console.WriteLine("  3 = get Reviews");
var choice = Console.ReadLine();

if (choice == "1")
{
    var db = provider.GetRequiredService<SteamDatabaseManager>();
    db.AddPlayer("76561198309816250");
    var service = provider.GetRequiredService<PlayerSyncService>();
    await service.SyncPlayerLibrariesAsync();
}
else if (choice == "2")
{
    var service = provider.GetRequiredService<BadgeSyncService>();
    await service.SyncPlayerBadgesAsync();
}
else if (choice == "3")
{
    await RunGetreviewAsync(provider);
}


provider.GetRequiredService<SteamDatabaseManager>().Dispose();


static async Task RunGetreviewAsync(IServiceProvider provider)
{
    var db = provider.GetRequiredService<SteamDatabaseManager>();
    var app = db.GetRandomUnsearchedGame();
    var appId = app.game_id;

    if (string.IsNullOrEmpty(appId))
    {
        Console.WriteLine("Invalid AppID.");
        return;
    }

    Console.Write("Max pages to visit? (blank =no limit): ");
    var maxPagesInput = Console.ReadLine()?.Trim();
    int? maxPages = int.TryParse(maxPagesInput, out var p) ? p : null;

    var scraper = provider.GetRequiredService<ok2>();
    var total = await scraper.GeteviewsAsync(appId, new ExtractionOptions
    {
        MaxPages = maxPages,
        DelayMs = 300
    });

    Console.WriteLine($"run complete. {total} new players added to DB.");
}