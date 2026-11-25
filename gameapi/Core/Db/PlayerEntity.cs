using Microsoft.Data.Sqlite;

namespace Core.Db;

public class Player
{
    public string Id { get; set; } = "";
    public string? Personaname { get; set; }
    public string? AvatarFull { get; set; }
    public string? CountryCode { get; set; }
    public int Visibility { get; set; }
    public long? TimeCreated{ get; set; }
    public long? LastLogoff { get; set; }
    public int SteamLevel { get; set; }
    public int Xp { get; set; }
    public bool Search { get; set; }
    public DateTime? Time { get; set; }
    
    public static Player MapPlayer(SqliteDataReader reader)
    {
        return new Player
        {
            Id           = reader.GetString(reader.GetOrdinal("player_id")),
            Personaname  = GetNullableString(reader, "Personaname"),
            AvatarFull   = GetNullableString(reader, "avatar_full"),
            CountryCode  = GetNullableString(reader, "country_code"),
            Visibility   = reader.GetInt32(reader.GetOrdinal("visibility")),
            TimeCreated  = GetNullableLong(reader, "time_created"),
            LastLogoff   = GetNullableLong(reader, "last_logoff"),
            SteamLevel   = reader.GetInt32(reader.GetOrdinal("steam_lvl")),
            Xp           = reader.GetInt32(reader.GetOrdinal("xp")),
            Search       = reader.GetBoolean(reader.GetOrdinal("search")),
            Time         = GetNullableDateTime(reader, "time")
        };
    }

    private static string? GetNullableString(SqliteDataReader reader, string column)
    {
        int i = reader.GetOrdinal(column);
        return reader.IsDBNull(i) ? null : reader.GetString(i);
    }

    private static long? GetNullableLong(SqliteDataReader reader, string column)
    {
        int i = reader.GetOrdinal(column);
        return reader.IsDBNull(i) ? null : reader.GetInt64(i);
    }

    private static DateTime? GetNullableDateTime(SqliteDataReader reader, string column)
    {
        int i = reader.GetOrdinal(column);
        if (reader.IsDBNull(i)) return null;

        var text = reader.GetString(i);
        return DateTime.TryParse(text, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt)
            ? dt
            : null;
    }
}