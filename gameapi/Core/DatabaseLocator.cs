namespace Core;

public class DatabaseLocator
{
    private static readonly string[] Markers = [".git", "LICENSE"];

    public static string Root { get; } = FindRoot();

    public static string Db(string fileName, string? overrideDir = null)
    {
        var dir = string.IsNullOrWhiteSpace(overrideDir) ? Root : overrideDir;
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, fileName);
    }

    private static string FindRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            foreach (var m in Markers)
            {
                if(File.Exists(Path.Combine(dir.FullName, m)) ||
                   Directory.Exists(Path.Combine(dir.FullName, m)))
                    return dir.FullName;
            }
            dir = dir.Parent;
        }
        return AppContext.BaseDirectory;
    }
}