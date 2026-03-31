using CosUI.ViewModels;

namespace CosUI.Models;

public class ScriptFile : ViewModelBase
{
    private string _content = string.Empty;
    private bool _isDirty;

    public string Path { get; }
    public string Name => System.IO.Path.GetFileName(Path);

    public string Content
    {
        get => _content;
        set
        {
            if (!Set(ref _content, value)) return;
            _isDirty = true;
            OnPropertyChanged(nameof(IsDirty));
        }
    }

    public bool IsDirty => _isDirty;

    public ScriptFile(string path, string content = "")
    {
        Path = path;
        _content = content;
    }

    public void SetContent(string content)
    {
        _content = content;
        OnPropertyChanged(nameof(Content));
    }

    public void MarkClean()
    {
        _isDirty = false;
        OnPropertyChanged(nameof(IsDirty));
    }
}
