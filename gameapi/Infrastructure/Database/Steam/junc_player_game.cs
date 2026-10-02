using Core.Db;
using Microsoft.Data.Sqlite;

namespace Infrastructure.Database.Steam;

public class junc_player_game : BaseDatabase
{
    
    public junc_player_game(string filename) : base(filename) { }
    protected override void Init()
    {
        Execute(@"
        CREATE TABLE IF NOT EXISTS juncPlayerGame (
            player_id TEXT,
            game_id TEXT,
            hours_played TEXT,
            PRIMARY KEY(player_id, game_id),
            FOREIGN KEY(game_id) REFERENCES Games(game_id),
            FOREIGN KEY(player_id) REFERENCES PlayersIds(player_id)
        )");
    }

    public bool link(string playerId, string gameId, string hours, SqliteTransaction? tx = null)
    {
        try
        {
            using var cmd = _db.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = "INSERT OR IGNORE INTO juncPlayerGame (player_id,game_id,hours_played) VALUES (@player_id,@game_id,@hours_played)";
            cmd.Parameters.AddWithValue("@player_id", playerId);
            cmd.Parameters.AddWithValue("@game_id", gameId);
            cmd.Parameters.AddWithValue("@hours_played", hours);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch { return false; }
    }

    public bool linkLibrary(string playerId, string[] gameId, string[] hours)
    {
        int count = 0;
        using var tx = _db.BeginTransaction();
        foreach (var nw in gameId.Zip(hours, (a, b) => new { a, b }))
            if (link(playerId, nw.a, nw.b)) count++;
        tx.Commit();
        return count == gameId.Length;
    }

    public List<string>? getLibrary(string playerId)
    {
        var result = new List<string>();
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "SELECT game_id FROM juncPlayerGame WHERE player_id=@playerId";
        cmd.Parameters.AddWithValue("@playerId",playerId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(reader.GetString(0));
        }
        return result;
    }

    public string? getGameHours(string gameId)
    {
        var result = "";
        using var cmd = _db.CreateCommand();
        cmd.CommandText = "SELECT hours_played FROM juncPlayerGame WHERE game_id=@gameId";
        cmd.Parameters.AddWithValue("@gameId", gameId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result +=(reader.GetString(0));
        }
        return result;
    }

}