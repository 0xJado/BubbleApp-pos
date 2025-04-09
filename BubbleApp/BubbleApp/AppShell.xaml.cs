using BubbleAppNew.OrderConfirmationPage;

namespace BubbleAppNew;

public partial class AppShell : Shell
{
    public AppShell()
    {
        Routing.RegisterRoute(nameof(OrderPage), typeof(OrderPage));

        InitializeComponent();
    }
}