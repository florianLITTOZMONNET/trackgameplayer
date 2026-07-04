namespace Core.Db;

public class BadgeEntity
{
    public string badge_id { get; set; } = "";
    public string? Appid { get; set; }
    public string? CommuId { get; set; }
    public int Foil { get; set; }      // 0 = normal, 1 = foil
    public int Scarcity { get; set; }
}