namespace Core.Db;

public class Player
{
    public string Id { get; set; } = "";
    public string? Personaname { get; set; }
    public string? AvatarFull { get; set; }
    public string? CountryCode { get; set; }
    public int Visibility { get; set; }
    public long TimeCreated { get; set; }
    public long LastLogoff { get; set; }
    public int SteamLevel { get; set; }
    public int Xp { get; set; }
    public bool Search { get; set; }
    public DateTime? Time { get; set; }
}