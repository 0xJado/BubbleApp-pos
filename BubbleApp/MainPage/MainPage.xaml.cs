using BubbleApp.OrderSummaryPage;
using BubbleAppNew.CreatedMomentPage;
using BubbleAppNew.OrderConfirmationPage;
using CommunityToolkit.Maui.Views;
using BubbleLib.Models;
using Microsoft.Maui.Controls;


namespace BubbleAppNew.MainPage;

public partial class MainPage : ContentPage
{
    private readonly BubbleWaffleViewModel vm;

    public MainPage(BubbleWaffleViewModel viewModel)
    {
        InitializeComponent();
        vm = viewModel;
        BindingContext = vm;
    }

    private void OnCreateMomentClicked(object sender, EventArgs e)
    {
        var popup = new CreateMomentPopup();

        popup.MomentCreated += customOrder =>
        {
            vm.AddWaffleToOrder(customOrder);
        };
        
        this.ShowPopup(popup);
    }

    private async void OnGenerateQrCodeClicked(object sender, EventArgs e)
    {
        decimal solanaRate = await _coinGeckoService.GetSolanaEuroExchangeRateAsync();
        
        var orderedItems = _waffleService.OrderedItems.ToList();
        var euroPrice = _waffleService.GetTotalPrice();
        var storeWalletAddress = "0x1234567890abcdef1234567890abcdef12345678"; // Replace with actual wallet address
        
        var ocvm = new OrderConfirmationViewModel(
            orderedItems,
            euroPrice,
            solanaRate,
            storeWalletAddress);

        var orderConfirmationPage = new OrderConfirmationPage
        {
            BindingContext = ocvm
        };

        await Navigation.PushAsync(orderConfirmationPage);
    }
}