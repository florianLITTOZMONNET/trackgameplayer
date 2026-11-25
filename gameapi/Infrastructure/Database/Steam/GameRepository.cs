using Core.Db;

namespace Infrastructure.Database.Steam;

public class GameRepository : BaseDatabase
{
    public GameRepository(string filename) : base(filename) { }

    protected override void Init()
    {
        Execute(@"
            CREATE TABLE IF NOT EXISTS Games (
                game_id TEXT PRIMARY KEY,
                name TEXT NOT NULL UNIQUE,
                img_logo_url TEXT,
                has_community_visible_stats INTEGER NOT NULL DEFAULT 0,
                search INTEGER DEFAULT 0,
                time TEXT DEFAULT NULL
            )");
    }

    public bool AddGame(Game game)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = """
                          INSERT OR IGNORE INTO Games (game_id,name,img_logo_url,has_community_visible_stats)
                          VALUES (@id,@name,@img,@stats);
                          """;
        cmd.Parameters.AddWithValue("@id", game.game_id);
        cmd.Parameters.AddWithValue("@name", game.Name);
        cmd.Parameters.AddWithValue("@img", (object?)game.ImgLogoUrl ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@stats", game.HasCommunityVisibleStats ? 1 : 0);
        return cmd.ExecuteNonQuery() > 0;
    }

    public Game? GetById(string id)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "SELECT * FROM Games WHERE game_id=@id";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;
        return Game.MapGame(reader);
    }

    public List<string> GetAllIds()
    {
        var result = new List<string>();
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "SELECT game_id FROM Games";
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) result.Add(reader.GetString(0));
        return result;
    }

    public List<Game>? GetAll()
    {
        var result = new List<Game>();
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "SELECT * FROM Games";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            result.Add(Game.MapGame(reader));
        return result.Count > 0 ? result : null;
    }

    public void MarkSearched(string id)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "UPDATE Games SET search=1, time=DATE('now') WHERE game_id=@id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public List<Game>? GetUnsearched()
    {
        var result = new List<Game>();
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "SELECT * FROM Games WHERE search=0";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            result.Add(Game.MapGame(reader));
        return result.Count > 0 ? result : null;
    }

    public Game? GetRandomUnsearched()
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "SELECT * FROM Games WHERE time IS NULL ORDER BY RANDOM() LIMIT 1";
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;
        return Game.MapGame(reader);
    }

    public void RefreshSearchStatus()
    {
        Execute("UPDATE Games SET search=0, time=NULL WHERE time <= DATE('now', '-4 months')");
    }

    public bool DeleteId(string id)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "DELETE FROM Games WHERE game_id=@id";
        cmd.Parameters.AddWithValue("@id", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    public void Clear() => Execute("DELETE FROM Games");
}