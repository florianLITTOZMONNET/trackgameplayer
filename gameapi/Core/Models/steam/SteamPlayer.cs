namespace Core.Models.steam;

public class GetPlayerSummariesResponse
{
    public PlayerSummaryWrapper Response { get; set; } = new();
}

public class PlayerSummaryWrapper
{
    public List<SteamPlayer> Players { get; set; } = [];
}

public class SteamPlayer
{
    public string Steamid { get; set; } = "";
    public int Communityvisibilitystate { get; set; }
    public int? Profilestate { get; set; }
    public string Personaname { get; set; } = "";
    public string Profileurl { get; set; } = "";
    public string Avatar { get; set; } = "";
    public string Avatarmedium { get; set; } = "";
    public string Avatarfull { get; set; } = "";
    public string? Avatarhash { get; set; }
    public long? Lastlogoff { get; set; }
    public int Personastate { get; set; }
    public string? Realname { get; set; }
    public string? Primaryclanid { get; set; }
    public long? Timecreated { get; set; }
    public string? Loccountrycode { get; set; }
}