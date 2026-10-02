namespace Core.Models.steam;


public class LibraryWrapper { public LibraryStats? Response { get; set; } }

public class LibraryStats
{
    public int Game_count { get; set; }
    public List<GameOwnedStats> Games { get; set; } = [];
}

public class GameOwnedStats
{
    public int Appid { get; set; }
    public string Name { get; set; } = "";
    public long Playtime_forever { get; set; }
    public int Playtime_2weeks { get; set; }
    public string? Img_logo_url { get; set; }
    public bool Has_community_visible_stats { get; set; }
    public int Playtime_windows_forever { get; set; }
    public int Playtime_mac_forever { get; set; }
    public int Playtime_linux_forever { get; set; }
    public long Rtime_last_played { get; set; }
}