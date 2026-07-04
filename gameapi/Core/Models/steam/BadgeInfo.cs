namespace Core.Models.steam;

public class BadgesWrapper { public GetBadgesResponse? Response { get; set; } }

public class GetBadgesResponse
{
    public List<BadgeInfo> Badges { get; set; } = [];
    public int Player_xp { get; set; }
    public int Player_level { get; set; }
    public int Player_xp_needed_to_level_up { get; set; }
    public int Player_xp_needed_current_level { get; set; }
}

public class BadgeInfo
{
    public int Badgeid { get; set; }
    public int? Appid { get; set; }
    public int Level { get; set; }
    public long Completion_time { get; set; }
    public int Xp { get; set; }
    public string? Communityitemid { get; set; }
    public int? Border_color { get; set; }
    public int Scarcity { get; set; }
}