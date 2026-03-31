#nullable disable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class Cosmic
{
    public static event Action<int> OnClientConnected;
    public static event Action<int> OnClientDisconnected;
    public static event Action<int, int, string> OnOutput;
    public static event Action<int, int, int> OnUserInfo;

    private const int WsPort = 24950;
    private static HttpListener _listener;
    private static CancellationTokenSource _cts;
    private static readonly ConcurrentDictionary<int, Client> _clients = new();
    private static bool _initialized;
    private static readonly string[] _rbx = { "RobloxPlayerBeta", "Windows10Universal" };
    private static readonly HttpClient _http = new();
    private static Task _initTask;

    public static bool IsRunning { get; private set; }
    public static int ClientCount => _clients.Count;
    public static bool IsReady => _initTask?.IsCompleted == true;

    public static async Task Initialize()
    {
        if (_initialized) return;
        _initialized = true;
        string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Cosmic");
        Directory.CreateDirectory(dir);
        _initTask = Task.WhenAll(
            DL("https://auth.cosmic.best/files/dll", Path.Combine(dir, "Cosmic-Module.dll")),
            DL("https://auth.cosmic.best/files/injector", Path.Combine(dir, "Cosmic-Injector.exe")));
        _cts = new CancellationTokenSource();
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://127.0.0.1:{WsPort}/");
        _listener.Start();
        IsRunning = true;
        _ = Task.Run(() => AcceptLoop(_cts.Token));
        await _initTask.ConfigureAwait(false);
    }

    private static async Task DL(string url, string dest)
    {
        try { File.WriteAllBytes(dest, await _http.GetByteArrayAsync(url).ConfigureAwait(false)); }
        catch { }
    }

    public static void Shutdown()
    {
        if (!_initialized) return;
        IsRunning = false;
        _initialized = false;
        _cts?.Cancel();
        foreach (var c in _clients.Values)
            try { c.Sock.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).Wait(500); } catch { }
        _clients.Clear();
        try { _listener?.Stop(); } catch { }
    }

    public static Dictionary<int, int> Attach()
    {
        var r = new Dictionary<int, int>();
        foreach (int p in GetRobloxProcesses())
            try { r[p] = Attach(p); } catch { r[p] = -1; }
        return r;
    }

    public static int Attach(int pid)
    {
        string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Cosmic");
        string inj = Path.Combine(dir, "Cosmic-Injector.exe");
        if (!File.Exists(inj)) throw new FileNotFoundException(inj);
        using var p = Process.Start(new ProcessStartInfo
        {
            FileName = inj,
            Arguments = pid.ToString(),
            WorkingDirectory = dir,
            UseShellExecute = true
        });
        if (!p.WaitForExit(60000))
        {
            try { p.Kill(); } catch { }
            return -1;
        }
        return p.ExitCode;
    }

    public static List<int> GetRobloxProcesses()
    {
        var l = new List<int>();
        foreach (var n in _rbx)
        {
            try
            {
                foreach (var p in Process.GetProcessesByName(n))
                {
                    l.Add(p.Id);
                    p.Dispose();
                }
            }
            catch { }
        }
        return l;
    }

    public static List<int> GetClients() => _clients.Keys.ToList();

    public static int GetClientUserId(int pid) => _clients.TryGetValue(pid, out var c) ? c.UserId : 0;

    public static void KillClient(int pid)
    {
        if (_clients.TryGetValue(pid, out var c))
            try { c.Sock.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).Wait(500); } catch { }
        _clients.TryRemove(pid, out _);
        OnClientDisconnected?.Invoke(pid);
    }

    public static async Task WaitForReady()
    {
        if (_initTask != null) await _initTask.ConfigureAwait(false);
    }

    public static string GetAttachStatusMessage(int c) => c switch
    {
        6  => "Success",
        -1 => "Init failure",
        0  => "Failed to open",
        1  => "Version mismatch",
        2  => "DLL not found",
        3  => "Memory failed",
        4  => "PDB failed",
        5  => "Timeout",
        _  => $"Code {c}"
    };

    public static void Execute(string s) => Broadcast(s).GetAwaiter().GetResult();
    public static void Execute(int pid, string s) => SendTo(pid, s).GetAwaiter().GetResult();
    public static void SetMaxFps(int fps) => Setting("Max-Fps", fps.ToString());
    public static void SetUnlockFps(bool v) => Setting("Unlock-Fps", v ? "true" : "false");
    public static void SetConsoleRedirect(bool v) => Setting("Console-Redirect", v ? "true" : "false");
    public static void SetConsoleFilter(string filter) => Setting("Console-Filter", filter);
    public static void SetAutoExecute(bool v) => Setting("Auto-Execute", v ? "true" : "false");

    public static void SendSettingsTo(int pid, bool consoleRedir, string consoleFilter)
    {
        string r = consoleRedir ? "true" : "false";
        SendTo(pid, "{\"Name\":\"Console-Redirect\",\"Value\":\"" + r + "\"}").GetAwaiter().GetResult();
        if (!string.IsNullOrEmpty(consoleFilter))
            SendTo(pid, "{\"Name\":\"Console-Filter\",\"Value\":\"" + consoleFilter + "\"}").GetAwaiter().GetResult();
    }

    private class Client
    {
        public int Pid;
        public WebSocket Sock;
        public int UserId;
        public int GameId;
    }

    private static async Task AcceptLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested && IsRunning)
        {
            try
            {
                var ctx = await _listener.GetContextAsync().ConfigureAwait(false);
                if (!ctx.Request.IsWebSocketRequest)
                {
                    ctx.Response.StatusCode = 400;
                    ctx.Response.Close();
                    continue;
                }
                if (!int.TryParse(ctx.Request.Headers["process-id"], out int pid))
                {
                    ctx.Response.StatusCode = 400;
                    ctx.Response.Close();
                    continue;
                }
                var ws = await ctx.AcceptWebSocketAsync(null).ConfigureAwait(false);
                var c = new Client { Pid = pid, Sock = ws.WebSocket };
                _clients[pid] = c;
                OnClientConnected?.Invoke(pid);
                _ = Task.Run(() => Handle(c, ct));
            }
            catch (ObjectDisposedException) { break; }
            catch (HttpListenerException) { break; }
            catch { }
        }
    }

    private static async Task Handle(Client c, CancellationToken ct)
    {
        var buf = new byte[8192];
        try
        {
            while (c.Sock.State == WebSocketState.Open && !ct.IsCancellationRequested)
            {
                var r = await c.Sock.ReceiveAsync(new ArraySegment<byte>(buf), ct).ConfigureAwait(false);
                if (r.MessageType == WebSocketMessageType.Close) break;
                if (r.MessageType != WebSocketMessageType.Text) continue;

                string msg;
                if (r.EndOfMessage)
                {
                    msg = Encoding.UTF8.GetString(buf, 0, r.Count);
                }
                else
                {
                    using var ms = new MemoryStream();
                    ms.Write(buf, 0, r.Count);
                    while (!r.EndOfMessage)
                    {
                        r = await c.Sock.ReceiveAsync(new ArraySegment<byte>(buf), ct).ConfigureAwait(false);
                        ms.Write(buf, 0, r.Count);
                    }
                    msg = Encoding.UTF8.GetString(ms.ToArray());
                }
                Parse(c, msg);
            }
        }
        catch { }
        finally
        {
            _clients.TryRemove(c.Pid, out _);
            OnClientDisconnected?.Invoke(c.Pid);
            try { c.Sock.Dispose(); } catch { }
        }
    }

    private static void Parse(Client c, string m)
    {
        if (string.IsNullOrEmpty(m) || m[0] != '{') return;
        try
        {
            if (m.Contains("\"Status\"") && m.Contains("\"Message\""))
            {
                int s = JI(m, "Status");
                string msg = JS(m, "Message");
                if (msg != null) OnOutput?.Invoke(c.Pid, s, msg);
            }
            else if (m.Contains("\"UserId\""))
            {
                c.UserId = JI(m, "UserId");
                c.GameId = JI(m, "GameId");
                OnUserInfo?.Invoke(c.Pid, c.UserId, c.GameId);
            }
        }
        catch { }
    }

    private static async Task Broadcast(string msg)
    {
        var b = Encoding.UTF8.GetBytes(msg);
        var seg = new ArraySegment<byte>(b);
        var tasks = new List<Task>();
        foreach (var c in _clients.Values.ToArray())
        {
            if (c.Sock.State == WebSocketState.Open)
                tasks.Add(Task.Run(async () =>
                {
                    try { await c.Sock.SendAsync(seg, WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false); }
                    catch { }
                }));
        }
        if (tasks.Count > 0) await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private static async Task SendTo(int pid, string msg)
    {
        if (_clients.TryGetValue(pid, out var c) && c.Sock.State == WebSocketState.Open)
        {
            var b = Encoding.UTF8.GetBytes(msg);
            await c.Sock.SendAsync(new ArraySegment<byte>(b), WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false);
        }
    }

    private static void Setting(string n, object v)
    {
        string j = v is bool b
            ? $"{{\"Name\":\"{n}\",\"Value\":{(b ? "true" : "false")}}}"
            : $"{{\"Name\":\"{n}\",\"Value\":\"{v}\"}}";
        Broadcast(j).GetAwaiter().GetResult();
    }

    private static string JS(string json, string key)
    {
        string s = $"\"{key}\"";
        int i = json.IndexOf(s, StringComparison.Ordinal);
        if (i < 0) return null;
        i = json.IndexOf(':', i + s.Length);
        if (i < 0) return null;
        i = json.IndexOf('"', i + 1);
        if (i < 0) return null;
        int e = json.IndexOf('"', i + 1);
        if (e < 0) return null;
        return json.Substring(i + 1, e - i - 1)
            .Replace("\\n", "\n")
            .Replace("\\r", "\r")
            .Replace("\\t", "\t")
            .Replace("\\\"", "\"")
            .Replace("\\\\", "\\");
    }

    private static int JI(string json, string key)
    {
        string s = $"\"{key}\"";
        int i = json.IndexOf(s, StringComparison.Ordinal);
        if (i < 0) return 0;
        i = json.IndexOf(':', i + s.Length);
        if (i < 0) return 0;
        i++;
        while (i < json.Length && (json[i] == ' ' || json[i] == '\t')) i++;
        int st = i;
        while (i < json.Length && (char.IsDigit(json[i]) || json[i] == '-')) i++;
        return int.TryParse(json.Substring(st, i - st), out int v) ? v : 0;
    }
}
