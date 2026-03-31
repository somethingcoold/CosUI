using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using CosUI.Models;
using CosUI.Themes;
using CosUI.ViewModels;

namespace CosUI.Controls;

public partial class FileSidebar : UserControl
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(MainViewModel), typeof(FileSidebar),
            new PropertyMetadata(null, OnViewModelChanged));

    public MainViewModel? ViewModel
    {
        get => (MainViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private ScriptFile? _ctxFile;
    private TextBox? _renameBox;
    private ScriptFile? _renamingFile;
    private Canvas? _renameCanvas;

    public FileSidebar()
    {
        InitializeComponent();
    }

    private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sidebar = (FileSidebar)d;
        if (e.NewValue is MainViewModel vm)
        {
            sidebar.ScriptList.ItemsSource = vm.ScriptFiles;
            sidebar.AutoExecList.ItemsSource = vm.AutoExecFiles;
            sidebar.ClientList.ItemsSource = vm.ConnectedClients;
            vm.ConnectedClients.CollectionChanged += (_, _) => sidebar.UpdateClientCount();
            vm.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(MainViewModel.ActiveTab))
                    sidebar.SyncSelection();
            };
        }
    }

    private void UpdateClientCount()
    {
        if (ViewModel is null) return;
        var n = ViewModel.ConnectedClients.Count;
        ClientCountLabel.Text = n > 0 ? n.ToString() : "";
    }

    private void SyncSelection()
    {
        if (ViewModel is null) return;
        var activePath = ViewModel.ActiveTab?.Path;
        ScriptList.SelectedItem = activePath != null
            ? ScriptList.Items.OfType<ScriptFile>().FirstOrDefault(f => f.Path == activePath)
            : null;
        AutoExecList.SelectedItem = activePath != null
            ? AutoExecList.Items.OfType<ScriptFile>().FirstOrDefault(f => f.Path == activePath)
            : null;
    }

    private void ScriptList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ScriptList.SelectedItem is ScriptFile file && ViewModel is not null)
        {
            AutoExecList.SelectedItem = null;
            ViewModel.AddTab(file);
        }
    }

    private void AutoExecList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AutoExecList.SelectedItem is ScriptFile file && ViewModel is not null)
        {
            ScriptList.SelectedItem = null;
            ViewModel.AddTab(file);
        }
    }

    private void NewFile_Click(object sender, RoutedEventArgs e)
    {
        var name = ShowNewFilePopup("New Script");
        if (name is null) return;
        if (!name.EndsWith(".luau", StringComparison.OrdinalIgnoreCase) &&
            !name.EndsWith(".lua", StringComparison.OrdinalIgnoreCase))
            name += ".luau";
        ViewModel?.NewNamedFile(name);
    }

    private void NewAutoExecFile_Click(object sender, RoutedEventArgs e)
    {
        var name = ShowNewFilePopup("New AutoExec Script");
        if (name is null) return;
        if (!name.EndsWith(".luau", StringComparison.OrdinalIgnoreCase) &&
            !name.EndsWith(".lua", StringComparison.OrdinalIgnoreCase))
            name += ".luau";
        ViewModel?.NewNamedAutoExecFile(name);
    }

    private void ScriptList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (ScriptList.SelectedItem is ScriptFile file)
            BeginRename(file, ScriptList, ScriptRenameCanvas);
    }

    private void AutoExecList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (AutoExecList.SelectedItem is ScriptFile file)
            BeginRename(file, AutoExecList, AutoExecRenameCanvas);
    }

    private void ScriptList_RightClick(object sender, MouseButtonEventArgs e)
    {
        var file = HitTestFile(ScriptList, e.GetPosition(ScriptList));
        if (file is null) return;
        ScriptList.SelectedItem = file;
        _ctxFile = file;
        ShowContextMenu(e.GetPosition(RootGrid));
        e.Handled = true;
    }

    private void AutoExecList_RightClick(object sender, MouseButtonEventArgs e)
    {
        var file = HitTestFile(AutoExecList, e.GetPosition(AutoExecList));
        if (file is null) return;
        AutoExecList.SelectedItem = file;
        _ctxFile = file;
        ShowContextMenu(e.GetPosition(RootGrid));
        e.Handled = true;
    }

    private void ShowContextMenu(Point pos)
    {
        CtxPopup.HorizontalOffset = pos.X;
        CtxPopup.VerticalOffset = pos.Y;
        CtxPopup.IsOpen = true;
    }

    private void CtxRename_Click(object sender, MouseButtonEventArgs e)
    {
        CtxPopup.IsOpen = false;
        if (_ctxFile is null) return;
        var listBox = ScriptList.Items.Contains(_ctxFile) ? ScriptList : AutoExecList;
        var canvas = listBox == ScriptList ? ScriptRenameCanvas : AutoExecRenameCanvas;
        BeginRename(_ctxFile, listBox, canvas);
    }

    private void CtxDelete_Click(object sender, MouseButtonEventArgs e)
    {
        CtxPopup.IsOpen = false;
        if (_ctxFile is null || ViewModel is null) return;
        ViewModel.DeleteFile(_ctxFile);
        _ctxFile = null;
    }

    private void KillClient_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int pid)
            Cosmic.KillClient(pid);
    }

    private void Root_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (!CtxPopup.IsOpen) CancelRename();
    }

    private void BeginRename(ScriptFile file, ListBox listBox, Canvas canvas)
    {
        CancelRename();

        var container = listBox.ItemContainerGenerator.ContainerFromItem(file) as ListBoxItem;
        if (container is null) return;

        var pos = container.TranslatePoint(new Point(0, 0), canvas);
        var res = Application.Current.Resources;
        var primaryBlue = (Color)res[ThemeKeys.PrimaryBlue];

        var box = new TextBox
        {
            Text = Path.GetFileNameWithoutExtension(file.Path),
            Width = listBox.ActualWidth - 18,
            Height = container.ActualHeight,
            Background = (Brush)res[ThemeKeys.Brush(ThemeKeys.Surface)],
            Foreground = (Brush)res[ThemeKeys.Brush(ThemeKeys.TextPrimary)],
            CaretBrush = (Brush)res[ThemeKeys.Brush(ThemeKeys.AccentBlue)],
            BorderBrush = (Brush)res[ThemeKeys.Brush(ThemeKeys.PrimaryBlue)],
            BorderThickness = new Thickness(0, 0, 0, 1),
            FontSize = 12,
            Padding = new Thickness(16, 0, 0, 0),
            VerticalContentAlignment = VerticalAlignment.Center,
            SelectionBrush = new SolidColorBrush(Color.FromArgb(80, primaryBlue.R, primaryBlue.G, primaryBlue.B)),
        };

        Canvas.SetLeft(box, pos.X);
        Canvas.SetTop(box, pos.Y);
        box.KeyDown += RenameBox_KeyDown;
        box.LostFocus += RenameBox_LostFocus;

        canvas.IsHitTestVisible = true;
        canvas.Children.Add(box);
        _renameBox = box;
        _renamingFile = file;
        _renameCanvas = canvas;

        box.Focus();
        box.SelectAll();
    }

    private void RenameBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) { CommitRename(); e.Handled = true; }
        else if (e.Key == Key.Escape) { CancelRename(); e.Handled = true; }
    }

    private void RenameBox_LostFocus(object sender, RoutedEventArgs e)
        => CommitRename();

    private void CommitRename()
    {
        if (_renameBox is null || _renamingFile is null || ViewModel is null)
        {
            CancelRename();
            return;
        }
        var raw = _renameBox.Text.Trim();
        var file = _renamingFile;
        CancelRename();
        if (string.IsNullOrEmpty(raw)) return;
        if (!raw.EndsWith(".luau", StringComparison.OrdinalIgnoreCase) &&
            !raw.EndsWith(".lua", StringComparison.OrdinalIgnoreCase))
            raw += ".luau";
        if (raw != Path.GetFileName(file.Path))
            ViewModel.RenameFile(file, raw);
    }

    private void CancelRename()
    {
        if (_renameCanvas is not null)
        {
            _renameCanvas.Children.Clear();
            _renameCanvas.IsHitTestVisible = false;
        }
        _renameBox = null;
        _renamingFile = null;
        _renameCanvas = null;
    }

    private static ScriptFile? HitTestFile(ListBox listBox, Point pos)
    {
        var hit = VisualTreeHelper.HitTest(listBox, pos);
        if (hit?.VisualHit is null) return null;
        var el = hit.VisualHit as DependencyObject;
        while (el is not null)
        {
            if (el is ListBoxItem item) return item.DataContext as ScriptFile;
            el = VisualTreeHelper.GetParent(el);
        }
        return null;
    }

    private string? ShowNewFilePopup(string title)
    {
        var win = new Window
        {
            Width = 300, Height = 148,
            WindowStyle = WindowStyle.None,
            AllowsTransparency = true,
            Background = Brushes.Transparent,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ResizeMode = ResizeMode.NoResize,
            Owner = Window.GetWindow(this),
        };

        var res = Application.Current.Resources;
        var root = new Border
        {
            Background = (Brush)res["SurfaceBrush"],
            BorderBrush = (Brush)res["BorderActiveBrush"],
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(20, 16, 20, 16),
            Effect = new DropShadowEffect { BlurRadius = 24, ShadowDepth = 0, Opacity = 0.7, Color = Colors.Black },
        };

        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10) });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(14) });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var titleTb = new TextBlock
        {
            Text = title, FontSize = 12, FontWeight = FontWeights.SemiBold,
            Foreground = (Brush)res["TextPrimaryBrush"],
        };
        Grid.SetRow(titleTb, 0);

        var tb = new TextBox
        {
            Background = (Brush)res["BackgroundBrush"],
            Foreground = (Brush)res["TextPrimaryBrush"],
            BorderBrush = (Brush)res["BorderActiveBrush"],
            CaretBrush = (Brush)res["AccentBlueBrush"],
            SelectionBrush = (Brush)res["PrimaryBlueBrush"],
            BorderThickness = new Thickness(1),
            Padding = new Thickness(8, 6, 8, 6),
            FontSize = 12,
        };
        Grid.SetRow(tb, 2);

        var btnRow = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
        };
        var cancelBtn = new Button
        {
            Content = "Cancel", IsCancel = true, Width = 72, Margin = new Thickness(0, 0, 8, 0),
            Style = (Style)res["GhostBtnStyle"],
        };
        var okBtn = new Button
        {
            Content = "Create", IsDefault = true, Width = 72,
            Style = (Style)res["PrimaryBtnStyle"],
        };
        Grid.SetRow(btnRow, 4);

        string? result = null;
        okBtn.Click += (_, _) => { result = tb.Text.Trim(); win.DialogResult = true; };
        btnRow.Children.Add(cancelBtn);
        btnRow.Children.Add(okBtn);

        grid.Children.Add(titleTb);
        grid.Children.Add(tb);
        grid.Children.Add(btnRow);
        root.Child = grid;
        win.Content = root;

        win.Loaded += (_, _) => { tb.Focus(); };
        win.ShowDialog();
        return string.IsNullOrEmpty(result) ? null : result;
    }
}
