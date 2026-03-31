using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using CosUI.Models;
using CosUI.Services;
using CosUI.ViewModels;

namespace CosUI.Views;

public partial class MainWindow : Window
{
    private readonly FileService _files = new(App.ScriptsDir, App.AutoExecDir);
    private readonly MainViewModel _vm;
    private readonly SessionService _session = new(App.SessionPath);
    private System.Timers.Timer? _saveTimer;

    public MainWindow(string? openPath = null, bool restore = false)
    {
        InitializeComponent();
        _vm = new MainViewModel(_files);
        DataContext = _vm;

        _vm.ActiveTabChanged += OnActiveTabChanged;
        WireCosmicEvents();

        Loaded += (_, _) =>
        {
            if (restore) _restoreOnReady = true;
            else if (openPath is not null) _openPathOnReady = openPath;
        };
    }

    private bool _restoreOnReady;
    private string? _openPathOnReady;
    private ScriptFile? _prevTab;

    private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(5) };

    private void WireCosmicEvents()
    {
        Cosmic.OnClientConnected += pid => Dispatcher.BeginInvoke(() =>
        {
            UpdateClientStatus();
            _vm.ConnectedClients.Add(new ClientEntry(pid));
        });

        Cosmic.OnClientDisconnected += pid => Dispatcher.BeginInvoke(() =>
        {
            UpdateClientStatus();
            var entry = _vm.ConnectedClients.FirstOrDefault(c => c.Pid == pid);
            if (entry is not null) _vm.ConnectedClients.Remove(entry);
        });

        Cosmic.OnUserInfo += (pid, userId, _) =>
            Dispatcher.BeginInvoke(new Action(async () => await ResolveUsernameAsync(pid, userId)));

        Cosmic.OnOutput += (pid, status, msg) => Dispatcher.BeginInvoke(() =>
            _vm.AppendConsole(new ConsoleEntry(pid, status, msg, DateTime.Now)));
    }

    private async Task ResolveUsernameAsync(int pid, int userId)
    {
        if (userId <= 0) return;
        try
        {
            var json = await _http.GetStringAsync($"https://users.roblox.com/v1/users/{userId}");
            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("name", out var nameProp)) return;
            var username = nameProp.GetString();
            if (string.IsNullOrEmpty(username)) return;
            var entry = _vm.ConnectedClients.FirstOrDefault(c => c.Pid == pid);
            if (entry is not null) entry.Name = username;
        }
        catch { }
    }

    private void UpdateClientStatus()
    {
        _vm.ClientCount = Cosmic.ClientCount;
        ClientDot.IsActive = _vm.ClientCount > 0;
        StatusLabel.Text = _vm.ClientCount switch
        {
            0 => "",
            1 => "1 client",
            _ => $"{_vm.ClientCount} clients"
        };
    }

    private void Editor_Ready(object? sender, EventArgs e)
    {
        Editor.ContentChanged += OnEditorContentChanged;
        _vm.LoadScripts();
        if (_restoreOnReady) RestoreSession();
        else if (_openPathOnReady is not null) OpenFile(_openPathOnReady);
        else NewFile();
    }

    private void OnEditorContentChanged(string content)
    {
        if (_vm.ActiveTab is null) return;
        _vm.ActiveTab.SetContent(content);
        _files.SaveScript(_vm.ActiveTab.Path, content);
    }

    private void RestoreSession()
    {
        var data = _session.Load();
        if (data.Tabs.Count == 0) { NewFile(); return; }
        foreach (var tab in data.Tabs)
        {
            if (!File.Exists(tab.Path)) continue;
            // Use the same object already in ScriptFiles/AutoExecFiles so SyncSelection
            // reference equality works and the sidebar highlights correctly.
            var file = _vm.ScriptFiles.FirstOrDefault(f => f.Path == tab.Path)
                       ?? _vm.AutoExecFiles.FirstOrDefault(f => f.Path == tab.Path);
            if (file is null) continue;
            if (string.IsNullOrEmpty(file.Content))
                file.SetContent(_files.ReadScript(tab.Path));
            _vm.AddTab(file);
        }
        if (_vm.Tabs.Count == 0) { NewFile(); return; }
        var active = data.ActiveTabPath is not null
            ? _vm.Tabs.FirstOrDefault(t => t.Path == data.ActiveTabPath) ?? _vm.Tabs[0]
            : _vm.Tabs[0];
        _vm.ActiveTab = active;
        Editor.SetContent(active.Content);
        var tabEntry = data.Tabs.FirstOrDefault(t => t.Path == active.Path);
        if (tabEntry?.ScrollLine > 0) Editor.SetScroll(tabEntry.ScrollLine);
    }

    private void OpenFile(string path)
    {
        if (!File.Exists(path)) { NewFile(); return; }
        var file = new ScriptFile(path, _files.ReadScript(path));
        _vm.AddTab(file);
        Editor.SetContent(file.Content);
    }

    private void NewFile()
    {
        var file = _vm.NewFile();
        Editor.SetContent("");
    }

    private void OnActiveTabChanged(ScriptFile? file)
    {
        // Flush the tab we're switching AWAY from, not the new one.
        // _vm.ActiveTab is already the new tab when this fires, so we track
        // the previous tab ourselves to avoid overwriting the new file with stale content.
        if (_prevTab is not null)
        {
            var current = Editor.CurrentContent;
            _prevTab.SetContent(current);
            _files.SaveScript(_prevTab.Path, current);
        }
        _prevTab = file;

        if (file is null) { Editor.SetContent(""); FileNameLabel.Text = ""; return; }
        if (string.IsNullOrEmpty(file.Content) && File.Exists(file.Path))
            file.SetContent(_files.ReadScript(file.Path));
        Editor.SetContent(file.Content);
        FileNameLabel.Text = file.Name;
    }

    private void Window_Deactivated(object sender, EventArgs e)
    {
        // Flush current content to disk immediately on focus loss
        OnEditorContentChanged(Editor.CurrentContent);

        _saveTimer?.Stop();
        _saveTimer?.Dispose();
        _saveTimer = new System.Timers.Timer(2000) { AutoReset = false };
        _saveTimer.Elapsed += (_, _) => Dispatcher.BeginInvoke(() => _ = SaveSessionAsync());
        _saveTimer.Start();
    }

    private async Task SaveSessionAsync()
    {
        _saveTimer?.Stop();
        var tabs = _vm.Tabs.ToList();
        var entries = new List<TabEntry>();
        foreach (var tab in tabs)
        {
            int scroll = 0;
            if (tab == _vm.ActiveTab) scroll = await Editor.GetScrollAsync();
            entries.Add(new TabEntry { Path = tab.Path, ScrollLine = scroll });
        }
        _session.Save(new SessionData
        {
            Tabs = entries,
            ActiveTabPath = _vm.ActiveTab?.Path
        });
    }

    private bool _closing;
    private async void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_closing) return;
        e.Cancel = true;
        _closing = true;
        _saveTimer?.Stop();
        _saveTimer?.Dispose();
        _saveTimer = null;
        // Flush current content — CurrentContent is always live from Monaco pushes
        OnEditorContentChanged(Editor.CurrentContent);
        await SaveSessionAsync();
        Closing -= Window_Closing;
        try { Close(); }
        catch (InvalidOperationException) { }
    }

    private async void Execute_Click(object sender, RoutedEventArgs e)
    {
        var script = await Editor.GetContentAsync();
        if (_vm.ActiveTab is not null)
        {
            _files.SaveScript(_vm.ActiveTab.Path, script);
            _vm.ActiveTab.SetContent(script);
        }
        await Task.Run(() => Cosmic.Execute(script));
    }

    private async void Attach_Click(object sender, RoutedEventArgs e)
    {
        BtnAttach.IsEnabled = false;
        BtnAttach.Content = "Attaching...";
        var results = await Task.Run(() => Cosmic.Attach());
        if (results.Count == 0)
        {
            _vm.AppendConsole(new ConsoleEntry(0, 2, "No Roblox processes found", DateTime.Now));
        }
        else
        {
            foreach (var kvp in results)
            {
                var msg = Cosmic.GetAttachStatusMessage(kvp.Value);
                _vm.AppendConsole(new ConsoleEntry(kvp.Key, kvp.Value == 6 ? 3 : 2, $"PID {kvp.Key}: {msg}", DateTime.Now));
            }
        }
        BtnAttach.Content = "Attach";
        BtnAttach.IsEnabled = true;
        ConsoleView.ScrollToEnd();
    }

    private void Clear_Click(object sender, RoutedEventArgs e) => _vm.ClearConsole();

    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed) DragMove();
    }

    private void Settings_Click(object sender, MouseButtonEventArgs e)
        => new SettingsOverlay { Owner = this }.ShowDialog();

    private void WinClose_Click(object sender, MouseButtonEventArgs e) => Close();
    private void WinMinimize_Click(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Minimized;
    private void WinMaximize_Click(object sender, MouseButtonEventArgs e)
        => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
}
