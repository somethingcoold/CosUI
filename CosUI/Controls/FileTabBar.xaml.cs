using System.Windows;
using System.Windows.Controls;
using CosUI.Models;
using CosUI.ViewModels;

namespace CosUI.Controls;

public partial class FileTabBar : UserControl
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(MainViewModel), typeof(FileTabBar),
            new PropertyMetadata(null, OnViewModelChanged));

    public MainViewModel? ViewModel
    {
        get => (MainViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public FileTabBar()
    {
        InitializeComponent();
    }

    private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var bar = (FileTabBar)d;
        if (e.NewValue is MainViewModel vm)
        {
            bar.TabList.ItemsSource = vm.Tabs;
            vm.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(MainViewModel.ActiveTab))
                    bar.SyncSelection();
            };
        }
    }

    private void SyncSelection()
    {
        if (ViewModel is null) return;
        TabList.SelectedItem = ViewModel.ActiveTab;
    }

    private void TabList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TabList.SelectedItem is ScriptFile file && ViewModel is not null)
            ViewModel.ActiveTab = file;
    }

    private void NewFile_Click(object sender, RoutedEventArgs e)
        => ViewModel?.NewFile();

    private void CloseTab_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ScriptFile file)
            ViewModel?.CloseTab(file);
    }
}
