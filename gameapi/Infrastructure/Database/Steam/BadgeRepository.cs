using Core.Db;

namespace Infrastructure.Database.Steam;

public class BadgeRepository : BaseDatabase
{
    public BadgeRepository(string filename) : base(filename) { }

    protected override void Init()
    {
        Execute(@"
            CREATE TABLE IF NOT EXISTS Badge (
                badge_id TEXT PRIMARY KEY,
                appid TEXT DEFAULT NULL,
                commuId TEXT DEFAULT NULL,
                foil INTEGER NOT NULL DEFAULT 0,
                scarcity INTEGER NOT NULL,
                FOREIGN KEY (appid) REFERENCES Games(id)
            )");
    }

    public bool AddBadge(string id, string? appid, string? commuId, int foil, int scarcity)
    {
        try
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "INSERT OR IGNORE INTO Badge (badge_id,appid,commuId,foil,scarcity) VALUES (@id,@appid,@commuId,@foil,@scarcity)";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@appid", (object?)appid ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@commuId", (object?)commuId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@foil", foil);
            cmd.Parameters.AddWithValue("@scarcity", scarcity);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch (Exception e)
        {
            Console.WriteLine($"AddBadge failed for id={id}: {e.Message}");
            return false;
        }
    }

    public List<BadgeEntity>? GetAll()
    {
        var result = new List<BadgeEntity>();
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "SELECT * FROM Badge";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            result.Add(new BadgeEntity
            {
                badge_id = reader.GetString(0),
                Appid = reader.IsDBNull(1) ? null : reader.GetString(1),
                CommuId = reader.IsDBNull(2) ? null : reader.GetString(2),
                Foil = reader.GetInt32(3),
                Scarcity = reader.GetInt32(4)
            });
        return result.Count > 0 ? result : null;
    }

    public void Clear() => Execute("DELETE FROM Badge");
}