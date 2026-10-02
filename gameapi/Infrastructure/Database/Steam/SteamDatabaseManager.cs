using Core.Db;

namespace Infrastructure.Database.Steam;

public class SteamDatabaseManager : IDisposable
{
    private readonly PlayerRepository _players;
    private readonly GameRepository _games;
    private readonly BadgeRepository _badges;
    private readonly junc_player_game _juncPlayerGame;

    public SteamDatabaseManager(string dbPath = "steam.db")
    {
        _players = new PlayerRepository(dbPath);
        _games = new GameRepository(dbPath);
        _badges = new BadgeRepository(dbPath);
        _juncPlayerGame = new junc_player_game(dbPath);
    }

    // Players 
    public bool AddPlayer(string id) => _players.AddId(id);
    public int AddPlayers(List<string> ids) => _players.AddIds(ids);
    public List<string>? GetAllPlayers() => _players.GetAll();
    public List<Player>? GetUnsearchedPlayers() => _players.GetUnsearchedPlayers();
    public void MarkPlayerSearched(string id) => _players.MarkSearched(id);
    public void RefreshPlayerSearchStatus() => _players.RefreshSearchStatus();
    public bool RemovePlayer(string id) => _players.DeleteId(id);

    //Games
    public bool AddGame(string id, string name) => _games.AddGame(id, name);
    public List<Game>? GetAllGames() => _games.GetAll();
    public List<Game>? GetUnsearchedGames() => _games.GetUnsearched();
    public Game? GetRandomUnsearchedGame() => _games.GetRandomUnsearched();
    public void MarkGameSearched(string id) => _games.MarkSearched(id);
    public void RefreshGameSearchStatus() => _games.RefreshSearchStatus();
    
    public string? GameName(string id)=> _games.GetById(id)?.Name;

    // Badges 
    public bool AddBadge(string id, string? appid, string? commuId, int? foil, int scarcity)
        => _badges.AddBadge(id, appid, commuId, foil.HasValue ? (foil.Value > 0 ? 1 : 0) : 0, scarcity);
    public List<BadgeEntity>? GetAllBadges() => _badges.GetAll();
    
    
    //Junc-game-player

    public bool LinkGame(string pid, string gid, string hours) => _juncPlayerGame.link(pid, gid, hours);
    public bool LinkGames(string pid, string[] gid, string[] hours)=> _juncPlayerGame.linkLibrary(pid, gid, hours);
    public List<string>? GetLibrary(string pid)=> _juncPlayerGame.getLibrary(pid);
    public string? GetLibraryHours(string pid)=> _juncPlayerGame.getGameHours(pid);
    
    //  Junc-badge-player

    // Cleanup 
    public void ClearAll()
    {
        _badges.Clear();
        _games.Clear();
        _players.Clear();
    }

    public void Dispose()
    {
        _players.Dispose();
        _games.Dispose();
        _badges.Dispose();
    }
}