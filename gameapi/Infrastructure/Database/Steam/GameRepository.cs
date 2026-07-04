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
                search INTEGER DEFAULT 0,
                time TEXT DEFAULT NULL
            )");
    }

    public bool AddGame(string id, string name)
    {
        try
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "INSERT OR IGNORE INTO Games (game_id,name) VALUES (@id,@name)";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch { return false; }
    }

    public Game? GetById(string id)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "SELECT * FROM Games WHERE game_id=@id";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;
        return new Game { game_id = reader.GetString(0), Name = reader.GetString(1) };
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
            result.Add(new Game { game_id = reader.GetString(0), Name = reader.GetString(1) });
        return result.Count > 0 ? result : null;
    }

    public Game? GetRandomUnsearched()
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "SELECT * FROM Games WHERE time IS NULL ORDER BY RANDOM() LIMIT 1";
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;
        return new Game { game_id = reader.GetString(0), Name = reader.GetString(1) };
    }

    public List<Game>? GetAll()
    {
        var result = new List<Game>();
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "SELECT * FROM Games";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            result.Add(new Game { game_id = reader.GetString(0), Name = reader.GetString(1) });
        return result.Count > 0 ? result : null;
    }

    public void RefreshSearchStatus()
    {
        Execute("UPDATE Games SET search=0, time=NULL WHERE time <= DATE('now', '-4 months')");
    }

    public void Clear() => Execute("DELETE FROM Games");
}