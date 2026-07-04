namespace Core.Db;

public class Game
{
    public string game_id { get; set; } = "";
    public string Name { get; set; } = "";
    public bool Search { get; set; }
    public DateTime? Time { get; set; }
}