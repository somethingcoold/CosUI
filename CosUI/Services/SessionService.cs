using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CosUI.Services;

public class SessionService
{
    private readonly string _path;

    public SessionService(string path)
    {
        _path = path;
    }

    public void Save(SessionData data)
    {
        var dir = Path.GetDirectoryName(_path)!;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(_path, JsonSerializer.Serialize(data, _opts));
    }

    public SessionData Load()
    {
        try
        {
            if (!File.Exists(_path)) return new SessionData();
            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<SessionData>(json, _opts) ?? new SessionData();
        }
        catch
        {
            return new SessionData();
        }
    }

    private static readonly JsonSerializerOptions _opts = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}

public class SessionData
{
    public List<TabEntry> Tabs { get; set; } = [];
    public string? ActiveTabPath { get; set; }
}

public class TabEntry
{
    public string Path { get; set; } = string.Empty;
    public int ScrollLine { get; set; }
}
