using System.Windows;
using CosUI.ViewModels;

namespace CosUI.Views;

public partial class SettingsOverlay : Window
{
    public SettingsOverlay()
    {
        InitializeComponent();
        DataContext = new SettingsViewModel();
    }

    private void Close_Click(object s, RoutedEventArgs e) => Close();
}
