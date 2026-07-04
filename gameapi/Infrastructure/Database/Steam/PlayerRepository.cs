using Core.Db;

namespace Infrastructure.Database.Steam;

public class PlayerRepository : BaseDatabase
{
    public PlayerRepository(string filename) : base(filename) { }

    protected override void Init()
    {
        Execute(@"
            CREATE TABLE IF NOT EXISTS PlayersIds (
                player_id TEXT PRIMARY KEY,
                search INTEGER DEFAULT 0,
                time TEXT DEFAULT NULL
            )");
    }

    public bool AddId(string id)
    {
        try
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "INSERT OR IGNORE INTO PlayersIds (player_id) VALUES (@id)";
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch { return false; }
    }

    public int AddIds(List<string> ids)
    {
        int count = 0;
        using var tx = _db.BeginTransaction();
        foreach (var id in ids)
            if (AddId(id)) count++;
        tx.Commit();
        return count;
    }

    public Player? GetPlayer(string id)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "SELECT * FROM PlayersIds WHERE player_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;
        return new Player { Id = reader.GetString(0), Search = reader.GetBoolean(1) };
    }

    public List<string> GetAll()
    {
        var result = new List<string>();
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "SELECT player_id FROM PlayersIds";
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) result.Add(reader.GetString(0));
        return result;
    }

    public void MarkSearched(string id)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "UPDATE PlayersIds SET search=1, time=DATE('now') WHERE player_id=@id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public List<Player>? GetUnsearchedPlayers()
    {
        var result = new List<Player>();
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "SELECT * FROM PlayersIds WHERE search=0";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            result.Add(new Player { Id = reader.GetString(0), Search = false });
        return result.Count > 0 ? result : null;
    }

    public void RefreshSearchStatus()
    {
        Execute("UPDATE PlayersIds SET search=0, time=NULL WHERE time <= DATE('now', '-4 months')");
    }

    public bool DeleteId(string id)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "DELETE FROM PlayersIds WHERE player_id=@id";
        cmd.Parameters.AddWithValue("@id", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    public void Clear() => Execute("DELETE FROM PlayersIds");
}