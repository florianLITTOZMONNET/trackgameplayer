using Microsoft.Data.Sqlite;

namespace Infrastructure.Database;

public abstract class BaseDatabase :IDisposable
{
    protected readonly SqliteConnection _db;
    private readonly string _file;

    protected BaseDatabase(string file)
    {
        _file = file;
        _db = new SqliteConnection($"Data Source={file};");
        _db.Open();
        
        Execute("PRAGMA foreign_keys = ON");
        Execute("PRAGMA journal_mode = WAL");

        Init();
    }
    
    protected abstract void Init();

    protected void Execute(string sql, object? param = null)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = sql;
        if (param != null)
            foreach (var prop in param.GetType().GetProperties())
                cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(param) ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public void Backup(string backupPath)
    {
        using var dest = new SqliteConnection($"Data Source={backupPath}");
        dest.Open();
        _db.BackupDatabase(dest);
    }

    public string GetFileName() => _file;

    public void Dispose() => _db.Close();
    
}