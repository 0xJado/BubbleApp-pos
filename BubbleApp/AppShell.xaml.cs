using BubbleAppNew.OrderConfirmationPage;
using static Microsoft.Maui.Controls.Routing;

namespace BubbleAppNew;

public partial class AppShell : Shell
{
    public AppShell()
    {
        Routing.RegisterRoute(nameof(OrderPage), typeof(OrderPage));

        InitializeComponent();
    }
}