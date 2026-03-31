using System.Collections.ObjectModel;
using System.IO;
using CosUI.Services;

namespace CosUI.ViewModels;

public class WelcomeViewModel : ViewModelBase
{
    private readonly FileService _files;
    private readonly SessionService _session;
    private bool _serverRunning;
    private bool _autoExecOn;
    private int _openScriptCount;

    public ObservableCollection<RecentEntry> RecentFiles { get; } = [];

    public bool ServerRunning
    {
        get => _serverRunning;
        set => Set(ref _serverRunning, value);
    }

    public bool AutoExecOn
    {
        get => _autoExecOn;
        set => Set(ref _autoExecOn, value);
    }

    public int OpenScriptCount
    {
        get => _openScriptCount;
        set => Set(ref _openScriptCount, value);
    }

    public WelcomeViewModel(FileService files, SessionService session)
    {
        _files = files;
        _session = session;
        LoadRecents();
    }

    public void LoadRecents()
    {
        RecentFiles.Clear();
        var data = _session.Load();
        foreach (var tab in data.Tabs.Take(5))
        {
            if (File.Exists(tab.Path))
                RecentFiles.Add(new RecentEntry(tab.Path, tab.ScrollLine));
        }
    }

    public void ClearHistory()
    {
        _session.Save(new SessionData());
        RecentFiles.Clear();
    }
}

public record RecentEntry(string Path, int ScrollLine)
{
    public string Name => System.IO.Path.GetFileName(Path);
    public string Directory => System.IO.Path.GetDirectoryName(Path) ?? string.Empty;
}
