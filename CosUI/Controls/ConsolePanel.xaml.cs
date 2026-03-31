using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace CosUI.Controls;

public partial class ConsolePanel : UserControl
{
    public static readonly DependencyProperty EntriesProperty =
        DependencyProperty.Register(nameof(Entries), typeof(IEnumerable), typeof(ConsolePanel),
            new PropertyMetadata(null, OnEntriesChanged));

    public IEnumerable? Entries
    {
        get => (IEnumerable?)GetValue(EntriesProperty);
        set => SetValue(EntriesProperty, value);
    }

    public ConsolePanel()
    {
        InitializeComponent();
    }

    private static void OnEntriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var panel = (ConsolePanel)d;
        if (panel.ConsoleList is null) return;
        panel.ConsoleList.ItemsSource = panel.Entries;
    }

    public void ScrollToEnd()
    {
        if (ConsoleList.Items.Count > 0)
            ConsoleList.ScrollIntoView(ConsoleList.Items[^1]);
    }
}
