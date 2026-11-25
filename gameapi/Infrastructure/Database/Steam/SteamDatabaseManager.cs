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
    public bool AddPlayerId(string id) => _players.AddPlayer(id);
    public bool AddPlayer(Player playerInfo) => _players.AddPlayer(playerInfo);
    public int AddPlayers(List<Player> playersInfo) => _players.AddPlayers(playersInfo);
    public Player? GetPlayer(string id) => _players.GetPlayer(id);
    public List<string> GetAllId()=> _players.GetAll();
    public List<Player> GetAll() => _players.GetAllInfo();
    public List<Player>? GetUnsearchedPlayers() => _players.GetUnsearchedPlayers();
    public void MarkPlayerSearched(string id) => _players.MarkSearched(id);
    public void RefreshPlayerSearchStatus() => _players.RefreshSearchStatus();
    public bool RemovePlayer(string id) => _players.DeleteId(id);

    //Games
    public bool AddGame(Game game) => _games.AddGame(game);
    public List<Game>? GetAllGames() => _games.GetAll();
    public List<string> GetAllGameIds() => _games.GetAllIds();
    public List<Game>? GetUnsearchedGames() => _games.GetUnsearched();
    public Game? GetRandomUnsearchedGame() => _games.GetRandomUnsearched();
    public void MarkGameSearched(string id) => _games.MarkSearched(id);
    public void RefreshGameSearchStatus() => _games.RefreshSearchStatus();
    public bool RemoveGame(string id) => _games.DeleteId(id);
    
    public string? GameName(string id)=> _games.GetById(id)?.Name;

    // Badges 
    public bool AddBadge(string id, string? appid, string? commuId, int? foil, int scarcity)
        => _badges.AddBadge(id, appid, commuId, foil.HasValue ? (foil.Value > 0 ? 1 : 0) : 0, scarcity);
    public List<BadgeEntity>? GetAllBadges() => _badges.GetAll();
    
    
    //Junc-game-player

    public bool LinkGame(string pid, string gid, string hours) => _juncPlayerGame.link(pid, gid, hours);
    public bool LinkGames(string pid, string[] gid, string[] hours)=> _juncPlayerGame.linkLibrary(pid, gid, hours);
    public List<string>? GetLibrary(string pid)=> _juncPlayerGame.getLibrary(pid);
    public string? GetGameHours(string pid,string gid)=> _juncPlayerGame.getGameHours(pid,gid);
    
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