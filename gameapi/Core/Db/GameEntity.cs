namespace Core.Db;

public class Game
{
    public string game_id { get; set; } = "";
    public string Name { get; set; } = "";
    public int hours { get; set; } = 0;
    public bool Search { get; set; }
    public DateTime? Time { get; set; }
}