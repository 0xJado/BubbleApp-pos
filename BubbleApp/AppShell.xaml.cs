using static Microsoft.Maui.Controls.Routing;

namespace BubbleAppNew;

public partial class AppShell : Shell
{
    public AppShell()
    {
        Routing.RegisterRoute(nameof(OrderConfirmationPage), typeof(OrderConfirmationPage));

        InitializeComponent();
    }
}