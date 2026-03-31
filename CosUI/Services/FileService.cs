using System.IO;

namespace CosUI.Services;

public class FileService
{
    private readonly string _scriptsDir;
    private readonly string _autoExecDir;

    public FileService(string scriptsDir, string autoExecDir)
    {
        _scriptsDir = scriptsDir;
        _autoExecDir = autoExecDir;
    }

    public IReadOnlyList<string> ListScripts()
    {
        if (!Directory.Exists(_scriptsDir)) return [];
        return Directory.GetFiles(_scriptsDir, "*.luau")
            .Concat(Directory.GetFiles(_scriptsDir, "*.lua"))
            .OrderBy(f => f)
            .ToList();
    }

    public IReadOnlyList<string> ListAutoExecScripts()
    {
        if (!Directory.Exists(_autoExecDir)) return [];
        return Directory.GetFiles(_autoExecDir, "*.luau")
            .Concat(Directory.GetFiles(_autoExecDir, "*.lua"))
            .OrderBy(f => f)
            .ToList();
    }

    public string ReadScript(string path)
    {
        if (!File.Exists(path)) return string.Empty;
        return File.ReadAllText(path);
    }

    public void SaveScript(string path, string content)
    {
        var dir = System.IO.Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(path, content);
    }

    public string CreateScript()
    {
        Directory.CreateDirectory(_scriptsDir);
        var path = UniquePath(_scriptsDir, "new_script", ".luau");
        File.WriteAllText(path, string.Empty);
        return path;
    }

    public string CreateNamedScript(string fileName)
    {
        Directory.CreateDirectory(_scriptsDir);
        var path = UniquePath(_scriptsDir, Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName));
        File.WriteAllText(path, string.Empty);
        return path;
    }

    public string CreateAutoExecScript()
    {
        Directory.CreateDirectory(_autoExecDir);
        var path = UniquePath(_autoExecDir, "new_script", ".luau");
        File.WriteAllText(path, string.Empty);
        return path;
    }

    public string CreateNamedAutoExecScript(string fileName)
    {
        Directory.CreateDirectory(_autoExecDir);
        var path = UniquePath(_autoExecDir, Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName));
        File.WriteAllText(path, string.Empty);
        return path;
    }

    public string RenameScript(string oldPath, string newName)
    {
        var dir = Path.GetDirectoryName(oldPath)!;
        var newPath = Path.Combine(dir, newName);
        File.Move(oldPath, newPath);
        return newPath;
    }

    public void DeleteScript(string path)
    {
        if (File.Exists(path)) File.Delete(path);
    }

    public string AutoExecDir => _autoExecDir;

    private static string UniquePath(string dir, string baseName, string ext)
    {
        var path = Path.Combine(dir, baseName + ext);
        var i = 1;
        while (File.Exists(path))
            path = Path.Combine(dir, $"{baseName}_{i++}{ext}");
        return path;
    }
}
