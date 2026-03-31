using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;

namespace CosUI.Controls;

public partial class MonacoEditor : UserControl
{
    private bool _ready;
    private string _pendingContent = string.Empty;
    private int _pendingScrollLine = -1;
    private readonly Dictionary<string, TaskCompletionSource<string>> _contentRequests = new();
    private readonly Dictionary<string, TaskCompletionSource<int>> _scrollRequests = new();

    public event EventHandler? EditorReady;
    public event Action<string>? ContentChanged;

    /// <summary>Last content pushed from Monaco. Always current within the debounce window.</summary>
    public string CurrentContent => _pendingContent;

    public MonacoEditor()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        App.Theme.ThemeChanged += OnThemeChanged;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await WebView.EnsureCoreWebView2Async();
        WebView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

        var exeDir = Path.GetDirectoryName(Environment.ProcessPath)
                     ?? AppDomain.CurrentDomain.BaseDirectory;
        var resourcesDir = Path.Combine(exeDir, "Resources");

        if (Directory.Exists(resourcesDir))
        {
            WebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "cosui.local", resourcesDir, CoreWebView2HostResourceAccessKind.Allow);
            WebView.CoreWebView2.Navigate("https://cosui.local/editor.html");
        }
        else
        {
            WebView.CoreWebView2.NavigateToString(EditorHtml);
        }
    }

    private const string EditorHtml = """
        <!DOCTYPE html>
        <html>
        <head>
        <meta charset="UTF-8">
        <style>
          * { margin: 0; padding: 0; box-sizing: border-box; }
          html, body, #editor { width: 100%; height: 100%; overflow: hidden; }
          body { background: #07070f; }
        </style>
        </head>
        <body>
        <div id="editor"></div>
        <script>
        var require = { paths: { vs: 'https://cdn.jsdelivr.net/npm/monaco-editor@0.47.0/min/vs' } };
        </script>
        <script src="https://cdn.jsdelivr.net/npm/monaco-editor@0.47.0/min/vs/loader.js"></script>
        <script>
        require(['vs/editor/editor.main'], function() {
          var editor = monaco.editor.create(document.getElementById('editor'), {
            value: '',
            language: 'lua',
            theme: 'vs-dark',
            fontSize: 13,
            lineHeight: 23,
            fontFamily: 'Cascadia Code, Consolas, monospace',
            minimap: { enabled: false },
            scrollBeyondLastLine: false,
            automaticLayout: true,
            wordWrap: 'off',
            renderLineHighlight: 'none',
            lineNumbers: 'on',
            scrollbar: { vertical: 'visible', verticalScrollbarSize: 6, horizontal: 'hidden' }
          });

          window.chrome.webview.postMessage(JSON.stringify({ type: 'editorReady' }));

          var changeTimer = null;
          var suppressChange = false;
          editor.onDidChangeModelContent(function() {
            if (suppressChange) return;
            clearTimeout(changeTimer);
            changeTimer = setTimeout(function() {
              window.chrome.webview.postMessage(JSON.stringify({
                type: 'contentChanged', content: editor.getValue()
              }));
            }, 400);
          });

          window.chrome.webview.addEventListener('message', function(e) {
            var msg = JSON.parse(e.data);
            switch (msg.type) {
              case 'setContent':
                suppressChange = true;
                editor.setValue(msg.content || '');
                suppressChange = false;
                break;
              case 'getContent':
                window.chrome.webview.postMessage(JSON.stringify({
                  type: 'contentResult', requestId: msg.requestId, content: editor.getValue()
                }));
                break;
              case 'setScroll':
                editor.revealLineInCenter(msg.line || 1);
                break;
              case 'getScroll':
                window.chrome.webview.postMessage(JSON.stringify({
                  type: 'scrollResult', requestId: msg.requestId,
                  line: editor.getVisibleRanges()[0] ? editor.getVisibleRanges()[0].startLineNumber : 1
                }));
                break;
              case 'setTheme':
                applyTheme(msg.theme);
                break;
            }
          });

          function applyTheme(t) {
            if (!t) return;
            monaco.editor.defineTheme('cosui', {
              base: 'vs-dark', inherit: true, rules: [],
              colors: {
                'editor.background': t.background || '#07070f',
                'editor.foreground': t.foreground || '#d0e0ff',
                'editorLineNumber.foreground': t.lineNumbers || '#1a2a55',
                'editor.lineHighlightBackground': t.lineHighlight || '#09091a',
                'editorCursor.foreground': t.cursor || '#1a66ff'
              }
            });
            monaco.editor.setTheme('cosui');
          }
        });
        </script>
        </body>
        </html>
        """;

    private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        var raw = e.TryGetWebMessageAsString();
        Dictionary<string, JsonElement>? msg;
        try { msg = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(raw); }
        catch { return; }
        if (msg is null) return;

        if (!msg.TryGetValue("type", out var typeProp)) return;
        var type = typeProp.GetString();

        Dispatcher.BeginInvoke(() =>
        {
            switch (type)
            {
                case "editorReady":
                    _ready = true;
                    if (!string.IsNullOrEmpty(_pendingContent))
                        PostJson(new { type = "setContent", content = _pendingContent });
                    if (_pendingScrollLine > 0)
                        PostJson(new { type = "setScroll", line = _pendingScrollLine });
                    SendCurrentTheme();
                    EditorReady?.Invoke(this, EventArgs.Empty);
                    break;

                case "contentChanged":
                    if (msg.TryGetValue("content", out var cc))
                    {
                        _pendingContent = cc.GetString() ?? string.Empty;
                        ContentChanged?.Invoke(_pendingContent);
                    }
                    break;

                case "contentResult":
                    if (msg.TryGetValue("requestId", out var cid) && msg.TryGetValue("content", out var cv))
                    {
                        var id = cid.GetString() ?? "";
                        if (_contentRequests.TryGetValue(id, out var tcs))
                        {
                            tcs.SetResult(cv.GetString() ?? "");
                            _contentRequests.Remove(id);
                        }
                    }
                    break;

                case "scrollResult":
                    if (msg.TryGetValue("requestId", out var sid) && msg.TryGetValue("line", out var lv))
                    {
                        var id = sid.GetString() ?? "";
                        if (_scrollRequests.TryGetValue(id, out var tcs))
                        {
                            tcs.SetResult(lv.GetInt32());
                            _scrollRequests.Remove(id);
                        }
                    }
                    break;
            }
        });
    }

    public void SetContent(string content)
    {
        _pendingContent = content;
        if (_ready)
            PostJson(new { type = "setContent", content });
    }

    public async Task<string> GetContentAsync()
    {
        if (!_ready) return _pendingContent;
        var id = Guid.NewGuid().ToString("N");
        var tcs = new TaskCompletionSource<string>();
        _contentRequests[id] = tcs;
        PostJson(new { type = "getContent", requestId = id });
        if (await Task.WhenAny(tcs.Task, Task.Delay(3000)) != tcs.Task)
        {
            _contentRequests.Remove(id);
            return _pendingContent;
        }
        return tcs.Task.Result;
    }

    public void SetScroll(int line)
    {
        if (_ready)
            PostJson(new { type = "setScroll", line });
        else
            _pendingScrollLine = line;
    }

    public Task<int> GetScrollAsync()
    {
        var id = Guid.NewGuid().ToString("N");
        var tcs = new TaskCompletionSource<int>();
        _scrollRequests[id] = tcs;
        PostJson(new { type = "getScroll", requestId = id });
        return tcs.Task;
    }

    private void SendCurrentTheme()
    {
        var t = App.Theme.Current;
        PostJson(new
        {
            type = "setTheme",
            theme = new
            {
                background    = ToHex(t.Background),
                foreground    = ToHex(t.TextPrimary),
                lineNumbers   = ToHex(t.TextMuted),
                lineHighlight = ToHex(t.Surface),
                cursor        = ToHex(t.PrimaryBlue)
            }
        });
    }

    private void OnThemeChanged(Themes.ITheme _) => Dispatcher.BeginInvoke(SendCurrentTheme);

    private void PostJson(object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        WebView.CoreWebView2?.PostWebMessageAsString(json);
    }

    private static string ToHex(System.Windows.Media.Color c)
        => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
}
