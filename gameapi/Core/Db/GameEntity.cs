using Core.Models.steam;
using Microsoft.Data.Sqlite;

namespace Core.Db;

public class Game
{
    public string game_id { get; set; } = "";
    public string Name { get; set; } = "";
    public string? ImgLogoUrl { get; set; }
    public bool HasCommunityVisibleStats { get; set; }
    public bool Search { get; set; }
    public DateTime? Time { get; set; }

    public static Game MapGame(SqliteDataReader reader)
    {
        return new Game
        {
            game_id                  = reader.GetString(reader.GetOrdinal("game_id")),
            Name                     = reader.GetString(reader.GetOrdinal("name")),
            ImgLogoUrl               = GetNullableString(reader, "img_logo_url"),
            HasCommunityVisibleStats = reader.GetBoolean(reader.GetOrdinal("has_community_visible_stats")),
            Search                   = reader.GetBoolean(reader.GetOrdinal("search")),
            Time                     = GetNullableDateTime(reader, "time")
        };
    }

    public static Game MapOwnedstat(GameOwnedStats stats)
    {
        return new Game
        {
            game_id = stats.Appid.ToString(),
            Name = stats.Name,
            ImgLogoUrl = stats.Img_logo_url,
            HasCommunityVisibleStats = stats.Has_community_visible_stats,
            Search = false,
            Time = null,
        };
    }

    private static string? GetNullableString(SqliteDataReader reader, string column)
    {
        int i = reader.GetOrdinal(column);
        return reader.IsDBNull(i) ? null : reader.GetString(i);
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