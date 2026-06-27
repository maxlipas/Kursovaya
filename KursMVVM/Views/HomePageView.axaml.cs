using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace KursMVVM;

public partial class HomePageView : UserControl
{
    public HomePageView()
    {
        InitializeComponent();
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}