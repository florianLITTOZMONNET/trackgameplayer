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
            PRIMARY KEY(player_id, game_id),
            FOREIGN KEY(game_id) REFERENCES Games(game_id),
            FOREIGN KEY(player_id) REFERENCES PlayersIds(player_id)
        )");
    }

    public bool link(string playerId, string gameId, SqliteTransaction? tx = null)
    {
        try
        {
            using var cmd = _db.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = "INSERT OR IGNORE INTO juncPlayerGame (player_id,game_id) VALUES (@player_id,@game_id)";
            cmd.Parameters.AddWithValue("@player_id", playerId);
            cmd.Parameters.AddWithValue("@game_id", gameId);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch { return false; }
    }

    public bool linkLibrary(string playerId, string[] gameId)
    {
        int count = 0;
        using var tx = _db.BeginTransaction();
        foreach (var id in gameId)
            if (link(playerId, id, tx)) count++;
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
}