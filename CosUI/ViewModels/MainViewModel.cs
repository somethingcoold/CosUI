using System.Collections.ObjectModel;
using CosUI.Models;
using CosUI.Services;

namespace CosUI.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly FileService _files;
    private ScriptFile? _activeTab;
    private bool _isAttaching;
    private int _clientCount;

    public ObservableCollection<ScriptFile> Tabs { get; } = [];
    public ObservableCollection<ScriptFile> ScriptFiles { get; } = [];
    public ObservableCollection<ScriptFile> AutoExecFiles { get; } = [];
    public ObservableCollection<ConsoleEntry> ConsoleEntries { get; } = [];
    public ObservableCollection<ClientEntry> ConnectedClients { get; } = [];

    public ScriptFile? ActiveTab
    {
        get => _activeTab;
        set
        {
            if (Set(ref _activeTab, value))
                ActiveTabChanged?.Invoke(value);
        }
    }

    public bool IsAttaching
    {
        get => _isAttaching;
        set => Set(ref _isAttaching, value);
    }

    public int ClientCount
    {
        get => _clientCount;
        set => Set(ref _clientCount, value);
    }

    public event Action<ScriptFile?>? ActiveTabChanged;

    public MainViewModel(FileService files)
    {
        _files = files;
    }

    public void AddTab(ScriptFile file)
    {
        if (Tabs.Any(t => t.Path == file.Path))
        {
            ActiveTab = Tabs.First(t => t.Path == file.Path);
            return;
        }
        Tabs.Add(file);
        ActiveTab = file;
    }

    public void CloseTab(ScriptFile file)
    {
        var idx = Tabs.IndexOf(file);
        Tabs.Remove(file);
        if (ActiveTab == file)
            ActiveTab = idx < Tabs.Count ? Tabs[idx] : Tabs.LastOrDefault();
    }

    public void AppendConsole(ConsoleEntry entry)
    {
        const int Cap = 2000;
        ConsoleEntries.Add(entry);
        if (ConsoleEntries.Count > Cap)
            ConsoleEntries.RemoveAt(0);
    }

    public void ClearConsole() => ConsoleEntries.Clear();

    public void LoadScripts()
    {
        ScriptFiles.Clear();
        foreach (var path in _files.ListScripts())
            ScriptFiles.Add(new ScriptFile(path));

        AutoExecFiles.Clear();
        foreach (var path in _files.ListAutoExecScripts())
            AutoExecFiles.Add(new ScriptFile(path));
    }

    public ScriptFile NewFile()
    {
        var path = _files.CreateScript();
        var f = new ScriptFile(path);
        ScriptFiles.Add(f);
        AddTab(f);
        return f;
    }

    public ScriptFile NewNamedFile(string fileName)
    {
        var path = _files.CreateNamedScript(fileName);
        var f = new ScriptFile(path);
        ScriptFiles.Add(f);
        AddTab(f);
        return f;
    }

    public ScriptFile NewAutoExecFile()
    {
        var path = _files.CreateAutoExecScript();
        var f = new ScriptFile(path);
        AutoExecFiles.Add(f);
        AddTab(f);
        return f;
    }

    public ScriptFile NewNamedAutoExecFile(string fileName)
    {
        var path = _files.CreateNamedAutoExecScript(fileName);
        var f = new ScriptFile(path);
        AutoExecFiles.Add(f);
        AddTab(f);
        return f;
    }

    public void DeleteActiveFile()
    {
        if (ActiveTab is null) return;
        DeleteFile(ActiveTab);
    }

    public void DeleteFile(ScriptFile file)
    {
        CloseTab(file);
        _files.DeleteScript(file.Path);
        var inScripts = ScriptFiles.FirstOrDefault(f => f.Path == file.Path);
        if (inScripts is not null) ScriptFiles.Remove(inScripts);
        var inAutoExec = AutoExecFiles.FirstOrDefault(f => f.Path == file.Path);
        if (inAutoExec is not null) AutoExecFiles.Remove(inAutoExec);
    }

    public void RenameFile(ScriptFile file, string newName)
    {
        var newPath = _files.RenameScript(file.Path, newName);
        var idx = Tabs.IndexOf(file);
        if (idx >= 0)
        {
            var renamed = new ScriptFile(newPath, file.Content);
            Tabs[idx] = renamed;
            if (ActiveTab == file) ActiveTab = renamed;
        }
        var sfIdx = ScriptFiles.IndexOf(file);
        if (sfIdx >= 0)
            ScriptFiles[sfIdx] = new ScriptFile(newPath);
        var aeIdx = AutoExecFiles.IndexOf(file);
        if (aeIdx >= 0)
            AutoExecFiles[aeIdx] = new ScriptFile(newPath);
    }
}
