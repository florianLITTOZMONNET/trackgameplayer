using Core.Db;
using Microsoft.EntityFrameworkCore.Sqlite.Storage.Internal;

namespace Infrastructure.Database.Steam;

public class PlayerRepository : BaseDatabase
{
    public PlayerRepository(string filename) : base(filename) { }

    protected override void Init()
    {
        Execute(@"
            CREATE TABLE IF NOT EXISTS PlayersIds (
                player_id TEXT PRIMARY KEY,
                Personaname TEXT,
                avatar_full STRING,
                country_code TEXT,
                visibility INTEGER NOT NULL DEFAULT 1,
                time_created INTEGER NOT NULL DEFAULT 0,
                last_logoff INTEGER DEFAULT 0,
                steam_lvl INTEGER DEFAULT 0,
                xp INTEGER dEFAULT 0,
                search INTEGER DEFAULT 0,
                time TEXT DEFAULT NULL
            )");
    }

    public bool AddPlayer(Player player)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = """
                          INSERT OR IGNORE INTO PlayersIds (player_id,personaname,avatar_full,contry_code,visibility,time_created,last_logoff) 
                          VALUES (@id,@n,@avatar,@contry_code,@visibility,@time_created,@last_logoff);
                          """;
        cmd.Parameters.AddWithValue("@id", player.Id);
        cmd.Parameters.AddWithValue("@n",player.Personaname);
        cmd.Parameters.AddWithValue("@avatar",player.AvatarFull);
        cmd.Parameters.AddWithValue("@contry_code",player.CountryCode);
        cmd.Parameters.AddWithValue("@visibility",player.Visibility);
        cmd.Parameters.AddWithValue("@time_created",player.TimeCreated ?? 0);
        cmd.Parameters.AddWithValue("@last_logoff",player.LastLogoff ?? 0);
        return cmd.ExecuteNonQuery() > 0;
    }

    public bool AddPlayer(string id)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "SELECT * FROM PlayersIds WHERE player_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        return cmd.ExecuteNonQuery() > 0;

    }
    
    public int AddPlayers(List<Player> players)
    {
        int count = 0;
        using var cmd = _db.CreateCommand();
        foreach (var player in players)
        {
            if (AddPlayer(player))
            {
                count++;
            }
        }
        return count;
    }

    public Player? GetPlayer(string id)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "SELECT * FROM PlayersIds WHERE player_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;
        return Player.MapPlayer(reader);
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

    public List<Player> GetAllInfo()
    {
        var result = new List<Player>();
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "SELECT * FROM PlayersIds";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(Player.MapPlayer(reader));
        }
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
            result.Add(Player.MapPlayer(reader));
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