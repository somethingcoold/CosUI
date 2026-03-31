using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CosUI.Controls;

public partial class StatusDot : UserControl
{
    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(StatusDot),
            new PropertyMetadata(false, OnIsActiveChanged));

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public StatusDot()
    {
        InitializeComponent();
    }

    private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((StatusDot)d).UpdateAnimation();

    private void UpdateAnimation()
    {
        var anim = (Storyboard)Resources["PulseAnim"];
        if (IsActive)
            anim.Begin();
        else
            anim.Stop();
    }
}
