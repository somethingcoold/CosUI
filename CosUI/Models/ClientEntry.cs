using CosUI.ViewModels;

namespace CosUI.Models;

public class ClientEntry : ViewModelBase
{
    private string _name;

    public int Pid { get; }

    public string Name
    {
        get => _name;
        set => Set(ref _name, value);
    }

    public ClientEntry(int pid)
    {
        Pid = pid;
        _name = $"PID {pid}";
    }
}
